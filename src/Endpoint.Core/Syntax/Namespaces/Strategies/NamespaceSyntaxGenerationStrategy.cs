// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Logging;
using System.Text;

namespace Endpoint.Core.Syntax.Namespaces.Strategies;

public class NamespaceGenerationStrategy : GenericSyntaxGenerationStrategy<NamespaceModel>
{
    private readonly ILogger<NamespaceGenerationStrategy> _logger;

    public NamespaceGenerationStrategy(ILogger<NamespaceGenerationStrategy> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<string> GenerateAsync(ISyntaxGenerator generator, NamespaceModel model)
    {
        _logger.LogInformation("Generating syntax. {name}", model.Name);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"namespace {model.Name};");

        return sb.ToString();
    }
}