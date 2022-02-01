﻿using Endpoint.Application.Services;
using Endpoint.Application.ValueObjects;

namespace Endpoint.Application.Builders
{
    public interface IApiFileBuilder
    {
        public void BuildProgram(Models.Settings settings);
        public void BuildLaunchSettings(Models.Settings settings);
    }

    public class ApiFileBuilder : IApiFileBuilder
    {

        protected readonly ICommandService _commandService;
        protected readonly ITemplateProcessor _templateProcessor;
        protected readonly ITemplateLocator _templateLocator;
        protected readonly IFileSystem _fileSystem;

        public ApiFileBuilder(
            ICommandService commandService,
            ITemplateProcessor templateProcessor,
            ITemplateLocator templateLocator,
            IFileSystem fileSystem)
        {
            _commandService = commandService;
            _templateProcessor = templateProcessor;
            _templateLocator = templateLocator;
            _fileSystem = fileSystem;
        }

        public void BuildLaunchSettings(Models.Settings settings)
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

            _fileSystem.WriteAllLines($@"{settings.ApiDirectory}/launchSettings.json", contents);

        }

        public void BuildProgram(Models.Settings settings)
        {
            var template = _templateLocator.Get(nameof(ProgramBuilder));

            var tokens = new TokensBuilder()
                .With(nameof(settings.InfrastructureNamespace), (Token)settings.InfrastructureNamespace)
                .With("Directory", (Token)settings.ApiDirectory)
                .With("Namespace", (Token)settings.ApiNamespace)
                .With("DbContext", (Token)settings.DbContextName)
                .Build();

            var contents = _templateProcessor.Process(template, tokens);

            _fileSystem.WriteAllLines($@"{settings.ApiDirectory}/Program.cs", contents);
        }
    }
}
