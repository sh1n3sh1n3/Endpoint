using Endpoint.Cli.Services;
using Endpoint.Cli.ValueObjects;

namespace Endpoint.Cli.Builders.CSharp
{
    public class ControllerEndpointBuilder: BuilderBase<ControllerEndpointBuilder>
    {
        public ControllerEndpointBuilder(
            ICommandService commandService,
            ITemplateProcessor templateProcessor,
            ITemplateLocator templateLocator,
            IFileSystem fileSystem):base(commandService, templateProcessor, templateLocator, fileSystem)
        { }

        
        public void Build()
        {
            var template = _templateLocator.Get(nameof(ControllerEndpointBuilder));

            var tokens = new TokensBuilder()
                .With(nameof(_rootNamespace), _rootNamespace)
                .With(nameof(_directory), _directory)
                .With(nameof(_namespace), _namespace)
                .Build();

            var contents = _templateProcessor.Process(template, tokens);

            _fileSystem.WriteAllLines($@"{_directory.Value}/ControllerEndpoint.cs", contents);
        }
    }
}