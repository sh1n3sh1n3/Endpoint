﻿using Endpoint.Application.Builders;
using Endpoint.Application.Enums;
using Endpoint.SharedKernal.Models;
using Endpoint.SharedKernal.Services;
using Endpoint.SharedKernal.ValueObjects;
using System.IO;

namespace Endpoint.Application.Services
{
    public class ApiFileService : BaseFileService, IApiFileService
    {
        public ApiFileService(
            ICommandService commandService,
            ITemplateProcessor templateProcessor,
            ITemplateLocator templateLocator,
            IFileSystem fileSystem)
            : base(commandService, templateProcessor, templateLocator, fileSystem)
        { }

        public void Build(Settings settings)
        {
            _removeDefaultFiles(settings);

            _commandService.Start("dotnet new tool-manifest", settings.ApiDirectory);

            _commandService.Start("dotnet tool install --version 6.0.7 Swashbuckle.AspNetCore.Cli", settings.ApiDirectory);

            var csProjFilePath = $"{settings.ApiDirectory}{Path.DirectorySeparatorChar}{settings.ApiNamespace}.csproj";

            CsProjService.AddGenerateDocumentationFile(csProjFilePath);

            CsProjService.AddEndpointPostBuildTargetElement(csProjFilePath);

            foreach (var resource in settings.Resources)
            {
                ControllerBuilder.Default(settings, resource.Name, _fileSystem);
            }

            _buildLaunchSettings(settings);
            
            ProgramBuilder.Build(settings, _templateLocator, _templateProcessor, _fileSystem);

            _buildStartup(settings);
            _buildAppSettings(settings);
            _buildDependencies(settings);
            _installNugetPackages(settings);

        }

        private void _buildAppSettings(Endpoint.SharedKernal.Models.Settings settings)
        {
            var template = _templateLocator.Get(nameof(AppSettingsBuilder));

            var tokens = new TokensBuilder()
                .With("RootNamespace", (Token)settings.RootNamespace)
                .With("Directory", (Token)settings.ApiDirectory)
                .With("Namespace", (Token)settings.ApiNamespace)
                .Build();

            var contents = _templateProcessor.Process(template, tokens);

            _fileSystem.WriteAllLines($@"{settings.ApiDirectory}/appsettings.json", contents);
        }

        private void _buildStartup(Endpoint.SharedKernal.Models.Settings settings)
        {
            var template = _templateLocator.Get(nameof(StartupBuilder));

            var tokens = new TokensBuilder()
                .With("RootNamespace", (Token)settings.RootNamespace)
                .With("Directory", (Token)settings.ApiDirectory)
                .With("Namespace", (Token)settings.ApiNamespace)
                .With("DbContext", (Token)settings.DbContextName)
                .Build();

            var contents = _templateProcessor.Process(template, tokens);

            _fileSystem.WriteAllLines($@"{settings.ApiDirectory}/Startup.cs", contents);
        }

        private void _buildDependencies(Endpoint.SharedKernal.Models.Settings settings)
        {
            var template = _templateLocator.Get(nameof(DependenciesBuilder));

            var tokens = new TokensBuilder()
                .With("RootNamespace", (Token)settings.RootNamespace)
                .With("Directory", (Token)settings.ApiDirectory)
                .With("Namespace", (Token)settings.ApiNamespace)
                .With("DbContext", (Token)settings.DbContextName)
                .With(nameof(settings.ApplicationNamespace), (Token)settings.ApplicationNamespace)
                .With(nameof(settings.ApiNamespace), (Token)settings.ApiNamespace)
                .With(nameof(settings.DomainNamespace), (Token)settings.DomainNamespace)
                .With(nameof(settings.InfrastructureNamespace), (Token)settings.InfrastructureNamespace)
                .Build();

            var contents = _templateProcessor.Process(template, tokens);

            _fileSystem.WriteAllLines($@"{settings.ApiDirectory}/Dependencies.cs", contents);
        }

        private void _buildLaunchSettings(Endpoint.SharedKernal.Models.Settings settings)
        {
            var template = _templateLocator.Get(nameof(LaunchSettingsBuilder));

            var tokens = new TokensBuilder()
                .With(nameof(settings.RootNamespace), (Token)settings.RootNamespace)
                .With("Directory", (Token)settings.ApiDirectory)
                .With("Namespace", (Token)settings.ApiNamespace)
                .With(nameof(settings.Port), (Token)$"{settings.Port}")
                .With(nameof(settings.SslPort), (Token)$"{settings.SslPort}")
                .With("ProjectName", (Token)settings.ApiNamespace)
                .With("LaunchUrl", (Token)"")
                .Build();

            var contents = _templateProcessor.Process(template, tokens);

            _fileSystem.WriteAllLines($@"{settings.ApiDirectory}/Properties/launchSettings.json", contents);
        }

        private void _createSubDirectories(Endpoint.SharedKernal.Models.Settings settings)
        {
            _commandService.Start($"mkdir {Constants.Folders.Behaviors}", settings.ApplicationDirectory);

            _commandService.Start($"mkdir {Constants.Folders.Logs}", settings.ApiDirectory);

        }

        public void _removeDefaultFiles(Endpoint.SharedKernal.Models.Settings settings)
        {
            _commandService.Start($"rimraf WeatherForecast.cs", $@"{settings.ApiDirectory}");
            _commandService.Start($@"rimraf Controllers\WeatherForecastController.cs", $@"{settings.ApiDirectory}");
        }

        private void _installNugetPackages(Endpoint.SharedKernal.Models.Settings settings)
        {
            _commandService.Start($"dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 5.0.10", $@"{settings.ApiDirectory}");
            _commandService.Start($"dotnet add package MediatR.Extensions.Microsoft.DependencyInjection  --version 9.0.0", $@"{settings.ApiDirectory}");
            _commandService.Start($"dotnet add package Swashbuckle.AspNetCore --version 6.2.2", $@"{settings.ApiDirectory}");
            _commandService.Start($"dotnet add package Swashbuckle.AspNetCore.Swagger --version 6.2.2", $@"{settings.ApiDirectory}");
            _commandService.Start($"dotnet add package Serilog.AspNetCore --version 4.1.0", $@"{settings.ApiDirectory}");
            _commandService.Start($"dotnet add package Serilog.Sinks.Seq --version 5.1.1", $@"{settings.ApiDirectory}");
            _commandService.Start($"dotnet add package SerilogTimings --version 2.3.0", $@"{settings.ApiDirectory}");
            _commandService.Start($"dotnet add package Swashbuckle.AspNetCore.Annotations --version 6.2.2", $@"{settings.ApiDirectory}");
            _commandService.Start($"dotnet add package Swashbuckle.AspNetCore.Newtonsoft --version 6.2.2", $@"{settings.ApiDirectory}");
            _commandService.Start($"dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson --version 5.0.9", $@"{settings.ApiDirectory}");
        }

        public void BuildAdditionalResource(string additionalResource, Settings settings)
        {
            new ClassBuilder($"{((Token)additionalResource).PascalCase}Controller", new Endpoint.SharedKernal.Services.Context(), _fileSystem)
                .WithDirectory($"{settings.ApiDirectory}{Path.DirectorySeparatorChar}Controllers")
                .WithUsing("System.Net")
                .WithUsing("System.Threading")
                .WithUsing("System.Threading.Tasks")
                .WithUsing($"{settings.ApplicationNamespace}")
                .WithUsing("MediatR")
                .WithUsing("System")
                .WithUsing("Microsoft.AspNetCore.Mvc")
                .WithUsing("Microsoft.Extensions.Logging")
                .WithUsing("Swashbuckle.AspNetCore.Annotations")
                .WithUsing("System.Net.Mime")
                .WithNamespace($"{settings.ApiNamespace}.Controllers")
                .WithAttribute(new AttributeBuilder().WithName("ApiController").Build())
                .WithAttribute(new AttributeBuilder().WithName("Route").WithParam("\"api/[controller]\"").Build())
                .WithAttribute(new AttributeBuilder().WithName("Produces").WithParam("MediaTypeNames.Application.Json").Build())
                .WithAttribute(new AttributeBuilder().WithName("Consumes").WithParam("MediaTypeNames.Application.Json").Build())
                .WithDependency("IMediator", "mediator")
                .WithDependency($"ILogger<{((Token)additionalResource).PascalCase}Controller>", "logger")
                .WithMethod(new MethodBuilder().WithEndpointType(EndpointType.GetById).WithResource(additionalResource).WithAuthorize(false).Build())
                .WithMethod(new MethodBuilder().WithEndpointType(EndpointType.Get).WithResource(additionalResource).WithAuthorize(false).Build())
                .WithMethod(new MethodBuilder().WithEndpointType(EndpointType.Create).WithResource(additionalResource).WithAuthorize(false).Build())
                .WithMethod(new MethodBuilder().WithEndpointType(EndpointType.Page).WithResource(additionalResource).WithAuthorize(false).Build())
                .WithMethod(new MethodBuilder().WithEndpointType(EndpointType.Update).WithResource(additionalResource).WithAuthorize(false).Build())
                .WithMethod(new MethodBuilder().WithEndpointType(EndpointType.Delete).WithResource(additionalResource).WithAuthorize(false).Build())
                .Build();
        }
    }
}
