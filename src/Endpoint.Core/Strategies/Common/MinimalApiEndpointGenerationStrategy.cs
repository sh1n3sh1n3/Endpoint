﻿using Endpoint.Core.Abstractions;
using Endpoint.Core.Models.Artifacts.Solutions;
using Endpoint.Core.Models.Git;
using Endpoint.Core.Services;
using System.IO;

namespace Endpoint.Core.Strategies.Common;

public class MinimalApiEndpointGenerationStrategy : ArtifactGenerationStrategyBase<MinimalApiSolutionModel>
{
    private readonly IFileSystem _fileSystem;

    public MinimalApiEndpointGenerationStrategy(IServiceProvider serviceProvider,IFileSystem fileSystem)
        :base(serviceProvider)
    {
        _fileSystem = fileSystem;
    }

    public override void Create(IArtifactGenerationStrategyFactory artifactGenerationStrategyFactory, MinimalApiSolutionModel model, dynamic configuration = null)
    {
        var workspaceDirectory = $"{model.Directory}{Path.DirectorySeparatorChar}{model.Name}";

        _fileSystem.CreateDirectory(workspaceDirectory);

        artifactGenerationStrategyFactory.CreateFor(new GitModel(model.Name)
        {
            Directory = workspaceDirectory,
        });
    }
}