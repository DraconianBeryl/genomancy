namespace Genomancy.Core.Definitions;

public sealed record AlleleDefinition
{
    public AlleleDefinition(ResourceId id, string displayName = "")
    {
        Id = id;
        DisplayName = displayName;
    }

    public ResourceId Id { get; }

    public string DisplayName { get; }
}
