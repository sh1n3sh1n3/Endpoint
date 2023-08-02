
using Endpoint.Core.Options;


namespace Endpoint.Core.Strategies;

public class MinimalApiSettingsFileGenerationStrategy : ISolutionSettingsFileGenerationStrategy
{
    public bool? CanHandle(SolutionSettingsModel request) => request.Metadata.Contains(Constants.SolutionTemplates.Minimal);

    public SolutionSettingsModel Create(SolutionSettingsModel request)
    {

        return new();
    }
}

