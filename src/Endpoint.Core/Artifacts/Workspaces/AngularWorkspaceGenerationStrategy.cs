// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Endpoint.Core.Services;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Endpoint.Core.Artifacts.Workspaces;

public class AngularWorkspaceGenerationStrategy : GenericArtifactGenerationStrategy<AngularWorkspaceModel>
{
    private readonly ILogger<AngularWorkspaceGenerationStrategy> _logger;
    private readonly ICommandService _commandService;

    public AngularWorkspaceGenerationStrategy(ILogger<AngularWorkspaceGenerationStrategy> logger, ICommandService commandService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
    }

    public override async Task GenerateAsync(IArtifactGenerator artifactGenerator, AngularWorkspaceModel model, dynamic context = null)
    {
        _logger.LogInformation("Generating Angular Workspace. {name}", model.Name);

        _commandService.Start($"npm uninstall @angular/cli -g", model.RootDirectory);

        _commandService.Start($"npm install @angular/cli@{model.Version} -g", model.RootDirectory);

        _commandService.Start($"ng new {model.Name} --no-create-application", model.RootDirectory);

        _commandService.Start($"npm install @ngrx/component-store --force", Path.Combine(model.RootDirectory, model.Name));

        _commandService.Start($"npm install @ngrx/component --force", Path.Combine(model.RootDirectory, model.Name));
    }
}