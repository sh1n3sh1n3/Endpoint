﻿using Endpoint.Core.Abstractions;
using Endpoint.Core.Models.Artifacts.ApiProjectModels;
using Endpoint.Core.Models.Artifacts.Solutions;
using Endpoint.Core.Options;
using Endpoint.Core.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Endpoint.Core.Strategies.Common;

public class EndpointGenerationStrategyFactory : IEndpointGenerationStrategyFactory
{
    private readonly IList<IEndpointGenerationStrategy> _strategies;

    public EndpointGenerationStrategyFactory(
        ICommandService commandService,
        ISolutionFilesGenerationStrategy solutionFilesGenerationStrategy,
        ISharedKernelProjectFilesGenerationStrategy sharedKernelProjectFilesGenerationStrategy,
        IApplicationProjectFilesGenerationStrategy applicationProjectFilesGenerationStrategy,
        IInfrastructureProjectFilesGenerationStrategy infrastructureProjectFilesGenerationStrategy,
        IApiProjectFilesGenerationStrategy apiProjectFilesGenerationStrategy,
        IFileSystem fileSystem,
        ISolutionModelFactory solutionModelFactory,
        IMediator mediator,
        IArtifactGenerationStrategyFactory artifactGenerationStrategyFactory,
        IServiceProvider serviceProvider,
        ILogger<EndpointGenerationStrategyFactory> logger
        )
    {
        _strategies = new List<IEndpointGenerationStrategy>()
        {
            new EndpointGenerationStrategy(
                commandService,
                solutionFilesGenerationStrategy,
                sharedKernelProjectFilesGenerationStrategy,
                applicationProjectFilesGenerationStrategy,
                infrastructureProjectFilesGenerationStrategy,apiProjectFilesGenerationStrategy,
                mediator,
                logger),

            //new MinimalApiEndpointGenerationStrategy(serviceProvider, artifactGenerationStrategyFactory,commandService,fileSystem, solutionModelFactory)
        };
    }

    public void CreateFor(CreateEndpointOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var strategy = _strategies.Where(x => x.CanHandle(options)).OrderByDescending(x => x.Order).FirstOrDefault();

        if (strategy == null)
        {
            throw new InvalidOperationException("Cannot find a strategy for generation for the type " + options.Name);
        }

        strategy.Create(options);
    }
}