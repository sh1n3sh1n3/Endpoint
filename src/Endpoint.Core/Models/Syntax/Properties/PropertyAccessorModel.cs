﻿using System.Collections.Generic;

namespace Endpoint.Core.Models.Syntax.Properties;

public class PropertyAccessorModel
{
    public string AccessModifier { get; private set; }
    public PropertyAccessorType Type { get; private set; }

    public PropertyAccessorModel(string accessModifier, PropertyAccessorType classPropertyAccessorType)
        : this(classPropertyAccessorType)
    {
        AccessModifier = accessModifier;
    }

    public PropertyAccessorModel(PropertyAccessorType classPropertyAccessorType)
    {
        Type = classPropertyAccessorType;
    }

    private PropertyAccessorModel()
    {

    }

    public static PropertyAccessorModel Get => new PropertyAccessorModel(PropertyAccessorType.Get);

    public static PropertyAccessorModel PrivateSet => new PropertyAccessorModel("private", PropertyAccessorType.Set);

    public static List<PropertyAccessorModel> GetPrivateSet => new List<PropertyAccessorModel>() { Get, PrivateSet };

    public static bool IsGetPrivateSet(List<PropertyAccessorModel> accessors)
    {
        return true;
    }
}