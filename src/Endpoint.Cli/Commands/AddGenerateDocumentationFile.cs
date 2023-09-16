// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Endpoint.Core.Artifacts.Projects.Strategies;
using Endpoint.Core.Services;
using MediatR;

namespace Endpoint.Cli.Commands;

[Verb("generate-documentation-file-add")]
public class GenerateDocumentationFileAddRequest : IRequest
{
    [Option('d', Required = false)]
    public string Directory { get; set; } = Environment.CurrentDirectory;
}

public class GenerateDocumentationFileAddRequestHandler : IRequestHandler<GenerateDocumentationFileAddRequest>
{
    private readonly ISettingsProvider settingsProvider;
    private readonly IApiProjectFilesGenerationStrategy apiProjectFilesGenerationStrategy;

    public GenerateDocumentationFileAddRequestHandler(ISettingsProvider settingsProvider, IApiProjectFilesGenerationStrategy apiProjectFilesGenerationStrategy)
    {
        this.settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
        this.apiProjectFilesGenerationStrategy = apiProjectFilesGenerationStrategy ?? throw new System.ArgumentNullException(nameof(apiProjectFilesGenerationStrategy));
    }

    public async Task Handle(GenerateDocumentationFileAddRequest request, CancellationToken cancellationToken)
    {
        var settings = settingsProvider.Get(request.Directory);

        var apiCsProjPath = $"{settings.ApiDirectory}{Path.DirectorySeparatorChar}{settings.ApiNamespace}.csproj";

        apiProjectFilesGenerationStrategy.AddGenerateDocumentationFile(apiCsProjPath);
    }
}
