// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Endpoint.Core.Enums;
using Endpoint.Core.Models.Syntax.Attributes;
using Endpoint.Core.Models.Syntax.Classes;
using Endpoint.Core.Models.Syntax.Params;
using Endpoint.Core.Models.Syntax.Types;
using Endpoint.Core.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Endpoint.Core.Models.Syntax.Methods.Factories;

public class MethodModelFactory: IMethodModelFactory
{
    private readonly ILogger<MethodModelFactory> _logger;
    private readonly INamingConventionConverter _namingConventionConverter;

    public MethodModelFactory(
        ILogger<MethodModelFactory> logger,
        INamingConventionConverter namingConventionConverter)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _namingConventionConverter = namingConventionConverter ?? throw new ArgumentNullException(nameof(namingConventionConverter));
    }

    public MethodModel CreateControllerMethod(string name, string controller, RouteType routeType, string directory)
    {
        return routeType switch
        {
            _ => CreateDefaultControllerMethod(name, controller, routeType, directory)
        };
    }

    public MethodModel CreateDefaultControllerMethod(string name, string controller, RouteType routeType, string directory, string swaggerSummery = "", string swaggerDescription = "", string template = null, Dictionary<string, string> properties = null)
    {
        var nameTitleCase = _namingConventionConverter.Convert(NamingConvention.TitleCase,name);

        swaggerSummery = string.IsNullOrEmpty(swaggerSummery) ? nameTitleCase : swaggerSummery;

        swaggerDescription = string.IsNullOrEmpty(swaggerDescription) ? nameTitleCase : swaggerDescription;

        ClassModel controllerClassmodel = new ClassModel(controller);

        MethodModel methodModel = new MethodModel
        {
            ParentType = controllerClassmodel,
            Async = true,
            AccessModifier = AccessModifier.Public,
            Name = name
            
        };

        methodModel.ReturnType = TypeModel.CreateTaskOfActionResultOf($"{name}Response");

        methodModel.Params = new List<Params.ParamModel>
        {
            new ParamModel()
            {
                Type = new TypeModel($"{_namingConventionConverter.Convert(NamingConvention.PascalCase,name)}Request"),
                Name = "request",
                Attribute = new AttributeModel() { Name = "FromBody" }
            },
            ParamModel.CancellationToken
        };

        methodModel.Attributes.Add(new SwaggerOperationAttributeModel(swaggerSummery, swaggerDescription));

        methodModel.Attributes.Add(new()
        {
            Name = "HttpGet",
            Template = template,
            Properties = properties ?? new Dictionary<string, string>()
            {
                { "Name", _namingConventionConverter.Convert(NamingConvention.CamelCase,name) }
            }
        });

        methodModel.Attributes.Add(new ProducesResponseTypeAttributeModel("InternalServerError"));

        methodModel.Attributes.Add(new ProducesResponseTypeAttributeModel("BadRequest", "ProblemDetails"));

        methodModel.Attributes.Add(new ProducesResponseTypeAttributeModel("OK", $"{name}Response"));

        methodModel.Body = "return await _mediator.Send(request, cancellationToken);";

        return methodModel;
    }
}

