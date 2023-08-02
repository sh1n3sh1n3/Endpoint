// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Endpoint.Core.Abstractions;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;

namespace Endpoint.Core.Syntax.Types;

public class TypeSyntaxGenerationStrategy : SyntaxGenerationStrategyBase<TypeModel>
{
    private readonly ILogger<TypeSyntaxGenerationStrategy> _logger;
    public TypeSyntaxGenerationStrategy(
        IServiceProvider serviceProvider,
        ILogger<TypeSyntaxGenerationStrategy> logger)
        : base(serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override string Create(ISyntaxGenerator syntaxGenerator, TypeModel model, dynamic context = null)
    {
        _logger.LogInformation("Generating syntax for {0}.", model);

        var builder = new StringBuilder();

        builder.Append(model.Name);

        if (model.GenericTypeParameters.Count > 0)
        {
            builder.Append('<');

            builder.AppendJoin(',', model.GenericTypeParameters.Select(x => syntaxGenerator.CreateFor(x)));

            builder.Append('>');
        }

        return builder.ToString();
    }
}