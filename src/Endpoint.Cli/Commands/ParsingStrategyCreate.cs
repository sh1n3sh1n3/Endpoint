// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using CommandLine;
using Endpoint.Core.Artifacts;
using Endpoint.Core.Artifacts.Files.Factories;
using Endpoint.Core.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Endpoint.Cli.Commands;


[Verb("parsing-strategy-create")]
public class ParsingStrategyCreateRequest : IRequest
{
    [Option('n', "name")]
    public string Name { get; set; }

    [Option('i', "input")]
    public string Input { get; set; }


    [Option('d', Required = false)]
    public string Directory { get; set; } = System.Environment.CurrentDirectory;
}

public class ParsingStrategyCreateRequestHandler : IRequestHandler<ParsingStrategyCreateRequest>
{
    private readonly IArtifactGenerator _artifactGenerator;
    private readonly ILogger<ParsingStrategyCreateRequestHandler> _logger;
    private readonly IFileFactory _fileFactory;
    private readonly INamespaceProvider _namespaceProvider;

    public ParsingStrategyCreateRequestHandler(
        ILogger<ParsingStrategyCreateRequestHandler> logger,
        IArtifactGenerator artifactGenerator,
        IFileFactory fileFactory,
        INamespaceProvider namespaceProvider
        )
    {
        _artifactGenerator = artifactGenerator;
        _logger = logger;
        _fileFactory = fileFactory;
        _namespaceProvider = namespaceProvider;
    }

    public async Task Handle(ParsingStrategyCreateRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating Parsing Strategy: {name}", request.Name);

        var tokens = new TokensBuilder()
            .With("ModelName", request.Name)
            .With("InputName", request.Input)
            .With("Namespace", _namespaceProvider.Get(request.Directory))
            .Build();

        var model = _fileFactory.CreateTemplate("ParsingStrategy", $"{request.Name}{request.Input}ParsingStrategy", request.Directory, tokens: tokens);

        await _artifactGenerator.GenerateAsync(model);

    }
}