// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Endpoint.Core.Abstractions;

public abstract class SyntaxGenerationStrategyBase<T> : ISyntaxGenerationStrategy
{
    private readonly IServiceProvider _serviceProvider;

    public SyntaxGenerationStrategyBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public virtual bool CanHandle(object model, dynamic context = null) => model.GetType() == typeof(T);
    public string Create(dynamic model, dynamic context = null)
    {
        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            var syntaxGenerator = scope.ServiceProvider
                .GetRequiredService<ISyntaxGenerator>();

            return Create(syntaxGenerator, model, context);
        }
    }

    public abstract string Create(ISyntaxGenerator syntaxGenerator, T model, dynamic context = null);

    public async Task<string> CreateAsync(object model, dynamic context = null)
        => Create(model, context);

    public virtual int Priority => 0;
}

