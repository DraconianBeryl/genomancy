using Genomancy.Core.Definitions;

namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestDefinition
{
    public ResourceTestDefinition(
        ResourceTestId id,
        Func<SystemDefinitionBuilder> createSystemDefinition,
        IEnumerable<IResourceTestStep> steps,
        string displayName = "",
        IEnumerable<string>? tags = null)
    {
        ArgumentNullException.ThrowIfNull(createSystemDefinition);
        ArgumentNullException.ThrowIfNull(steps);

        Id = id;
        DisplayName = displayName;
        CreateSystemDefinition = createSystemDefinition;
        Steps = Array.AsReadOnly(steps.ToArray());
        Tags = Array.AsReadOnly((tags ?? [])
            .Select(tag => tag.Trim())
            .Where(tag => tag.Length > 0)
            .Order(StringComparer.Ordinal)
            .ToArray());

        if (Steps.Count == 0)
        {
            throw new ArgumentException("Resource test definition must contain at least one step.", nameof(steps));
        }
    }

    public ResourceTestId Id { get; }

    public string DisplayName { get; }

    public Func<SystemDefinitionBuilder> CreateSystemDefinition { get; }

    public IReadOnlyList<IResourceTestStep> Steps { get; }

    public IReadOnlyList<string> Tags { get; }
}
