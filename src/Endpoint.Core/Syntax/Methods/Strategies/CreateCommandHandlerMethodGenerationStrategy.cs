// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Endpoint.Core.Abstractions;
using Endpoint.Core.Services;
using Endpoint.Core.Syntax.Classes;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;

namespace Endpoint.Core.Syntax.Methods.Strategies;


public class CreateCommandHandlerMethodGenerationStrategy : MethodSyntaxGenerationStrategy
{
    private readonly INamingConventionConverter _namingConventionConverter;
    public CreateCommandHandlerMethodGenerationStrategy(
        IServiceProvider serviceProvider,
        INamingConventionConverter namingConventionConverter,
        ILogger<MethodSyntaxGenerationStrategy> logger)
        : base(serviceProvider, logger)
    {
        _namingConventionConverter = namingConventionConverter ?? throw new ArgumentNullException(nameof(namingConventionConverter));
    }
    public bool CanHandle(object model, dynamic context = null)
    {
        if (model is MethodModel methodModel && context?.Entity is ClassModel entity)
        {
            return methodModel.Name == "Handle" && methodModel.Params.FirstOrDefault().Type.Name.StartsWith($"Create");
        }

        return false;
    }

    public int Priority => int.MaxValue;

    public async Task<string> GenerateAsync(ISyntaxGenerator syntaxGenerator, MethodModel model, dynamic context = null)
    {
        var builder = new StringBuilder();

        var entity = context.Entity as ClassModel;

        var entityName = entity.Name;

        var entityNameCamelCase = _namingConventionConverter.Convert(NamingConvention.CamelCase, entityName);

        builder.AppendLine($"var {entityNameCamelCase} = new {((SyntaxToken)entityName).PascalCase()}();");

        builder.AppendLine("");

        builder.AppendLine($"_context.{((SyntaxToken)entityName).PascalCasePlural()}.Add({entityNameCamelCase});");

        builder.AppendLine("");

        foreach (var property in entity.Properties.Where(x => x.Name != $"{entityName}Id"))
        {
            builder.AppendLine($"{entityNameCamelCase}.{property.Name} = request.{property.Name};");
        }

        builder.AppendLine("");

        builder.AppendLine("await _context.SaveChangesAsync(cancellationToken);");

        builder.AppendLine("");

        builder.AppendLine("return new ()");

        builder.AppendLine("{");

        builder.AppendLine($"{((SyntaxToken)entityName).PascalCase()} = {entityNameCamelCase}.ToDto()".Indent(1));

        builder.AppendLine("};");

        model.Body = builder.ToString();

        return await base.GenerateAsync(syntaxGenerator, model);
    }
}

