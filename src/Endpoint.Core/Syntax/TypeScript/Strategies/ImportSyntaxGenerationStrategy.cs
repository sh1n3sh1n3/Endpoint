// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Endpoint.Core.Abstractions;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;

namespace Endpoint.Core.Syntax.TypeScript.Strategies;

public class ImportSyntaxGenerationStrategy : ISyntaxGenerationStrategy<ImportModel>
{
    private readonly ILogger<ImportSyntaxGenerationStrategy> _logger;
    public ImportSyntaxGenerationStrategy(
        IServiceProvider serviceProvider,
        ILogger<ImportSyntaxGenerationStrategy> logger)

    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int Priority => 0;

    public async Task<string> GenerateAsync(ISyntaxGenerator syntaxGenerator, ImportModel model, dynamic context = null)
    {
        _logger.LogInformation("Generating syntax for {0}.", model);

        var builder = new StringBuilder();

        builder.Append("import { ");

        builder.AppendJoin(',', model.Types.Select(x => x.Name));

        builder.Append(" } from \"");

        builder.Append(model.Module);

        builder.Append("\";");

        return builder.ToString();
    }
}
