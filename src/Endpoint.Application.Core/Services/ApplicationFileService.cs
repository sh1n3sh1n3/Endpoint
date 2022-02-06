﻿using Endpoint.Application.Builders;
using Endpoint.Application.Core;
using Endpoint.Application.Enums;
using Endpoint.SharedKernal;
using Endpoint.SharedKernal.Models;
using Endpoint.SharedKernal.Services;
using Endpoint.SharedKernal.ValueObjects;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Endpoint.Application.Services
{
    public class ApplicationFileService : BaseFileService, IApplicationFileService
    {
        public ApplicationFileService(ICommandService commandService, ITemplateProcessor templateProcessor, ITemplateLocator templateLocator, IFileSystem fileSystem)
            : base(commandService, templateProcessor, templateLocator, fileSystem)
        { }

        public void Build(Settings settings)
        {
            _removeDefaultFiles(settings.ApplicationDirectory);

            _createFolder($"Interfaces", settings.ApplicationDirectory);

            _createFolder($"Behaviors", settings.ApplicationDirectory);

            _createFolder($"Extensions", settings.ApplicationDirectory);

            DbContextInterfaceBuilder.Default(settings, _fileSystem);

            foreach (var resource in settings.Resources.Select(x => (Token)x))
            {
                _buildApplicationFilesForResource(settings, resource);
            }

            _commandService.Start($"dotnet add package FluentValidation --version 10.3.3", $@"{settings.ApplicationDirectory}");
            _commandService.Start($"dotnet add package MediatR  --version 9.0.0", $@"{settings.ApplicationDirectory}");

        }

        protected void _buildApplicationFilesForResource(Settings settings, Token resource)
        {
            var aggregateDirectory = $"{settings.ApplicationDirectory}{Path.DirectorySeparatorChar}AggregatesModel{Path.DirectorySeparatorChar}{resource.PascalCase}Aggregate";
            var commandsDirectory = $"{aggregateDirectory}{Path.DirectorySeparatorChar}Commands";
            var queriesDirectory = $"{aggregateDirectory}{Path.DirectorySeparatorChar}Queries";
            var context = new Context();

            _createFolder(commandsDirectory, settings.ApplicationDirectory);
            _createFolder(queriesDirectory, settings.ApplicationDirectory);

            new ClassBuilder(resource.PascalCase, new Context(), _fileSystem)
                .WithDirectory(aggregateDirectory)
                .WithUsing("System")
                .WithNamespace($"{settings.ApplicationNamespace}")
                .WithProperty(new PropertyBuilder().WithName($"{resource.PascalCase}Id").WithType("Guid").WithAccessors(new AccessorsBuilder().Build()).Build())
                .Build();

            new ClassBuilder($"{resource.PascalCase}Dto", new Context(), _fileSystem)
                .WithDirectory(aggregateDirectory)
                .WithUsing("System")
                .WithNamespace($"{settings.ApplicationNamespace}")
                .WithProperty(new PropertyBuilder().WithName($"{resource.PascalCase}Id").WithType("Guid").WithAccessors(new AccessorsBuilder().Build()).Build())
                .Build();

            new ClassBuilder($"{resource.PascalCase}Extensions", new Context(), _fileSystem)
                .WithDirectory(aggregateDirectory)
                .IsStatic()
                .WithUsing("System.Collections.Generic")
                .WithUsing("Microsoft.EntityFrameworkCore")
                .WithUsing("System.Linq")
                .WithUsing("System.Threading.Tasks")
                .WithUsing("System.Threading")
                .WithNamespace(settings.ApplicationNamespace)
                .WithMethod(new MethodBuilder()
                .IsStatic()
                .WithName("ToDto")
                .WithReturnType($"{resource.PascalCase}Dto")
                .WithPropertyName($"{resource.PascalCase}Id")
                .WithParameter(new ParameterBuilder(resource.PascalCase, resource.CamelCase, true).Build())
                .WithBody(new()
                {
                    "return new ()",
                    "{",
                    $"{resource.PascalCase}Id = {resource.CamelCase}.{resource.PascalCase}Id".Indent(1),
                    "};"
                })
                .Build())
                .WithMethod(new MethodBuilder()
                .IsStatic()
                .WithAsync(true)
                .WithName("ToDtosAsync")
                .WithReturnType($"Task<List<{resource.PascalCase}Dto>>")
                .WithPropertyName($"{resource.PascalCase}Id")
                .WithParameter(new ParameterBuilder($"IQueryable<{resource.PascalCase}>", resource.CamelCasePlural, true).Build())
                .WithParameter(new ParameterBuilder("CancellationToken", "cancellationToken").Build())
                .WithBody(new()
                {
                    $"return await {resource.CamelCasePlural}.Select(x => x.ToDto()).ToListAsync(cancellationToken);"
                })
                .Build())
                .WithMethod(new MethodBuilder()
                .IsStatic()
                .WithName("ToDtos")
                .WithReturnType($"List<{resource.PascalCase}Dto>")
                .WithPropertyName($"{resource.PascalCase}Id")
                .WithParameter(new ParameterBuilder($"IEnumerable<{resource.PascalCase}>", resource.CamelCasePlural, true).Build())
                .WithBody(new()
                {
                    $"return {resource.CamelCasePlural}.Select(x => x.ToDto()).ToList();"
                })
                .Build())
                .Build();

            new ClassBuilder($"{resource.PascalCase}Validator", new Context(), _fileSystem)
                .WithDirectory(aggregateDirectory)
                .WithBase(new TypeBuilder().WithGenericType("AbstractValidator", $"{resource.PascalCase}Dto").Build())
                .WithNamespace($"{settings.ApplicationNamespace}")
                .WithUsing("FluentValidation")
                .Build();

            new CreateBuilder(new Context(), _fileSystem)
                .WithDirectory(commandsDirectory)
                .WithDbContext(settings.DbContextName)
                .WithNamespace($"{settings.ApplicationNamespace}")
                .WithApplicationNamespace($"{settings.ApplicationNamespace}")
                .WithDomainNamespace($"{settings.DomainNamespace}")
                .WithEntity(resource.Value)
                .Build();

            new UpdateBuilder(new Context(), _fileSystem)
                .WithDirectory(commandsDirectory)
                .WithDbContext(settings.DbContextName)
                .WithNamespace($"{settings.ApplicationNamespace}")
                .WithApplicationNamespace($"{settings.ApplicationNamespace}")
                .WithDomainNamespace($"{settings.DomainNamespace}")
                .WithEntity(resource.Value)
                .Build();

            new RemoveBuilder(new Context(), _fileSystem)
                .WithDirectory(commandsDirectory)
                .WithDbContext(settings.DbContextName)
                .WithNamespace($"{settings.ApplicationNamespace}")
                .WithApplicationNamespace($"{settings.ApplicationNamespace}")
                .WithDomainNamespace($"{settings.DomainNamespace}")
                .WithEntity(resource.Value)
                .Build();

            new GetByIdBuilder(new Context(), _fileSystem)
                .WithDirectory(queriesDirectory)
                .WithDbContext(settings.DbContextName)
                .WithNamespace($"{settings.ApplicationNamespace}")
                .WithApplicationNamespace($"{settings.ApplicationNamespace}")
                .WithDomainNamespace($"{settings.DomainNamespace}")
                .WithEntity(resource.Value)
                .Build();

            new GetBuilder(new Context(), _fileSystem)
                .WithDirectory(queriesDirectory)
                .WithDbContext(settings.DbContextName)
                .WithNamespace($"{settings.ApplicationNamespace}")
                .WithApplicationNamespace($"{settings.ApplicationNamespace}")
                .WithDomainNamespace($"{settings.DomainNamespace}")
                .WithEntity(resource.Value)
                .Build();

            new GetPageBuilder(new Context(), _fileSystem)
                .WithDirectory(queriesDirectory)
                .WithDbContext(settings.DbContextName)
                .WithNamespace($"{settings.ApplicationNamespace}")
                .WithApplicationNamespace($"{settings.ApplicationNamespace}")
                .WithDomainNamespace($"{settings.DomainNamespace}")
                .WithEntity(resource.Value)
                .Build();

            _buildValidationBehavior(settings);
            _buildServiceCollectionExtensions(settings);
        }

        public void BuildAdditionalResource(string additionalResource, Settings settings)
        {
            DbContextInterfaceBuilder.Default(settings, _fileSystem);

            _buildApplicationFilesForResource(settings, (Token)additionalResource);
        }

        private void _buildValidationBehavior(Settings settings)
        {
            var template = _templateLocator.Get(nameof(ValidationBehaviorBuilder));

            var tokens = new TokensBuilder()
                .With(nameof(settings.ApplicationNamespace), (Token)settings.ApplicationNamespace)
                .With(nameof(settings.DomainNamespace), (Token)settings.DomainNamespace)
                .Build();

            var contents = _templateProcessor.Process(template, tokens);

            _fileSystem.WriteAllLines($@"{settings.ApplicationDirectory}{Path.DirectorySeparatorChar}Behaviors{Path.DirectorySeparatorChar}ValidationBehavior.cs", contents);
        }

        private void _buildServiceCollectionExtensions(Settings settings)
        {
            var template = _templateLocator.Get(nameof(ServiceCollectionExtensionsBuilder));

            var tokens = new TokensBuilder()
                .With(nameof(settings.ApplicationNamespace), (Token)settings.ApplicationNamespace)
                .With(nameof(settings.DomainNamespace), (Token)settings.DomainNamespace)
                .Build();

            var contents = _templateProcessor.Process(template, tokens);

            _fileSystem.WriteAllLines($@"{settings.ApplicationDirectory}{Path.DirectorySeparatorChar}Extensions{Path.DirectorySeparatorChar}ServiceCollectionExtensions.cs", contents);
        }


    }
}
