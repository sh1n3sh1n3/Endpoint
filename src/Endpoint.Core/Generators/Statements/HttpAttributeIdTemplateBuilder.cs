
using Endpoint.Core.Options;
using Endpoint.Core.Syntax;


namespace Endpoint.Core.Builders.Statements;

public static class HttpAttributeIdTemplateBuilder
{
    public static string Build(SettingsModel settings, string resourceName)
    {
        var idPropertyName = settings.IdFormat == IdPropertyFormat.Long ? $"{((SyntaxToken)resourceName).CamelCase}Id" : "id";
        var idDotNetType = settings.IdDotNetType == IdPropertyType.Guid ? "guid" : "int";
        return "{" + idPropertyName + ":" + idDotNetType + "}";
    }
}

