// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Endpoint.Core.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Endpoint.Core.Syntax.SpecFlow.Strategies;

public class SpecFlowScenarioSyntaxGenerationStrategy : ISyntaxGenerationStrategy<SpecFlowScenarioModel>
{
    private readonly ILogger<SpecFlowScenarioSyntaxGenerationStrategy> _logger;
    public SpecFlowScenarioSyntaxGenerationStrategy(
        IServiceProvider serviceProvider,
        ILogger<SpecFlowScenarioSyntaxGenerationStrategy> logger)

    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public int Priority { get; } = 0;

    public async Task<string> GenerateAsync(ISyntaxGenerator syntaxGenerator, SpecFlowScenarioModel model, dynamic context = null)
    {
        _logger.LogInformation("Generating syntax for {0}.", model);

        var builder = new StringBuilder();


        return builder.ToString();
    }
}
