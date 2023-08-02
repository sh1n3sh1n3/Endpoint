// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Endpoint.Core.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Endpoint.Core.Artifacts.Files;

public class ObjectFileModel<T> : FileModel
{
    public ObjectFileModel(T @object, List<UsingDirectiveModel> usings, string name, string directory, string extension)
        : base(name.Split('.').Last(), directory, extension)
    {
        Object = @object;
        Usings = usings;
    }

    public ObjectFileModel(T @object, string name, string directory, string extension)
    : base(name.Split('.').Last(), directory, extension)
    {
        Object = @object;
        Usings = new List<UsingDirectiveModel>();
    }

    public T Object { get; init; }
    public List<UsingDirectiveModel> Usings { get; set; }

    public string Namespace { get; set; }
}
