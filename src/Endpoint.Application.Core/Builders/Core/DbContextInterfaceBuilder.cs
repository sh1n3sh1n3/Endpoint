﻿using Endpoint.Application.Builders;
using Endpoint.Application.Enums;
using Endpoint.SharedKernal.Services;
using Endpoint.SharedKernal.ValueObjects;
using System.IO;
using System.Linq;

namespace Endpoint.Application.Core
{
    public class DbContextInterfaceBuilder
    {
        public static void Default(Endpoint.SharedKernal.Models.Settings settings, IFileSystem fileSystem)
        {
            var classBuilder = new ClassBuilder(settings.DbContextName, new Endpoint.SharedKernal.Services.Context(), fileSystem, "interface")
            .WithDirectory($"{settings.ApplicationDirectory}{Path.DirectorySeparatorChar}Interfaces")
            .WithUsing("Microsoft.EntityFrameworkCore")
            .WithUsing("System.Threading.Tasks")
            .WithUsing("System.Threading")
            .WithNamespace($"{settings.ApplicationNamespace}.Interfaces")
            .WithMethodSignature(new MethodSignatureBuilder()
            .WithAsync(false)
            .WithAccessModifier(AccessModifier.Inherited)
            .WithName("SaveChangesAsync")
            .WithReturnType(new TypeBuilder().WithGenericType("Task", "int").Build())
            .WithParameter(new ParameterBuilder("CancellationToken", "cancellationToken").Build()).Build());

            foreach (var resource in settings.Resources.Select(x => (Token)x.Name))
            {
                classBuilder.WithProperty(new PropertyBuilder().WithName(resource.PascalCasePlural).WithAccessModifier(AccessModifier.Inherited).WithType(new TypeBuilder().WithGenericType("DbSet", resource.PascalCase).Build()).WithAccessors(new AccessorsBuilder().WithGetterOnly().Build()).Build());
            }

            classBuilder.Build();
        }
    }
}
