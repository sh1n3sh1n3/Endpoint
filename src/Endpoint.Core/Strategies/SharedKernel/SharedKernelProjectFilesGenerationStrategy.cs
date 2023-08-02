
using Endpoint.Core.Options;
using Endpoint.Core.Syntax;
using System.IO;


namespace Endpoint.Core.Services;

public class SharedKernelProjectFilesGenerationStrategy : BaseProjectFilesGenerationStrategy, ISharedKernelProjectFilesGenerationStrategy
{
    public SharedKernelProjectFilesGenerationStrategy(
        ICommandService commandService,
        ITemplateProcessor templateProcessor,
        ITemplateLocator templateLocator,
        IFileSystem fileSystem) : base(commandService, templateProcessor, templateLocator, fileSystem)
    { }

    public void Build(SettingsModel settings)
    {
        _createFolder($@"{settings.DomainDirectory}{Path.DirectorySeparatorChar}Extensions", settings.DomainDirectory);

        _removeDefaultFiles(settings.DomainDirectory);

        _buildQueryableExtensions(settings);

        _buildResponseBase(settings);
    }

    private void _buildResponseBase(SettingsModel settings)
    {
        var template = _templateLocator.Get("ResponseBase");

        var tokens = new TokensBuilder()
            .With("RootNamespace", (SyntaxToken)settings.RootNamespace)
            .With("Directory", (SyntaxToken)settings.DomainDirectory)
            .With("Namespace", (SyntaxToken)settings.DomainNamespace)
            .Build();

        var contents = string.Join(Environment.NewLine, _templateProcessor.Process(template, tokens));

        _fileSystem.WriteAllText($@"{settings.DomainDirectory}/ResponseBase.cs", contents);

        _commandService.Start($"dotnet add package Microsoft.EntityFrameworkCore --version 6.0.2", $@"{settings.DomainDirectory}");
    }

    private void _buildQueryableExtensions(SettingsModel settings)
    {
        var template = _templateLocator.Get("QueryableExtensions");

        var tokens = new TokensBuilder()
            .With("RootNamespace", (SyntaxToken)settings.RootNamespace)
            .With("Directory", (SyntaxToken)settings.DomainDirectory)
            .With("Namespace", (SyntaxToken)settings.DomainNamespace)
            .With("DomainNamespace", (SyntaxToken)settings.DomainNamespace)
            .Build();

        var contents = string.Join(Environment.NewLine, _templateProcessor.Process(template, tokens));

        _fileSystem.WriteAllText($@"{settings.DomainDirectory}{Path.DirectorySeparatorChar}Extensions{Path.DirectorySeparatorChar}QueryableExtensions.cs", contents);
    }
}

