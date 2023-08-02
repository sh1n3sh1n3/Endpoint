// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Endpoint.Core.Syntax.Properties;
using Endpoint.Core.Syntax.Types;
using Endpoint.Core.Syntax;
using Endpoint.Core.Syntax.Properties;
using System.Linq;

namespace Endpoint.Core.Syntax.Entities.Legacy;

[Obsolete]
public class LegacyAggregatesModelFactory : ILegacyAggregatesModelFactory
{
    private readonly ISyntaxService _syntaxService;

    public LegacyAggregatesModelFactory(ISyntaxService syntaxService)
    {
        _syntaxService = syntaxService ?? throw new ArgumentNullException(nameof(syntaxService));
    }

    public LegacyAggregatesModel Create(string resource, string properties)
    {
        LegacyAggregatesModel model = new LegacyAggregatesModel(resource);

        var idPropertyName = _syntaxService.SyntaxModel.IdPropertyFormat == IdPropertyFormat.Short ? "Id" : $"{((SyntaxToken)resource).PascalCase}Id";

        var idDotNetType = _syntaxService.SyntaxModel.IdPropertyType == IdPropertyType.Int ? "int" : "Guid";

        model.IdPropertyName = idPropertyName;

        model.IdPropertyType = idDotNetType;

        model.Properties.Add(new PropertyModel(model, AccessModifier.Public, new TypeModel() { Name = idDotNetType }, idPropertyName, PropertyAccessorModel.GetPrivateSet, key: true));

        if (!string.IsNullOrWhiteSpace(properties))
        {
            foreach (var property in properties.Split(','))
            {
                var nameValuePair = property.Split(':');

                model.Properties.Add(new PropertyModel(model, AccessModifier.Public, new TypeModel() { Name = nameValuePair.ElementAt(1) }, nameValuePair.ElementAt(0), PropertyAccessorModel.GetPrivateSet));
            }
        }

        return model;
    }
}
