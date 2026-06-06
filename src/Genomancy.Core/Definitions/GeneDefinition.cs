using System.Collections.ObjectModel;

namespace Genomancy.Core.Definitions;

public sealed record GeneDefinition
{
    public GeneDefinition(
        ResourceId id,
        IEnumerable<ResourceId> alleleIds,
        IEnumerable<PolicyReference>? policyReferences = null,
        string displayName = "")
    {
        Id = id;
        AlleleIds = alleleIds.ToReadOnlyList();
        PolicyReferences = (policyReferences ?? []).ToReadOnlyList();
        DisplayName = displayName;
    }

    public ResourceId Id { get; }

    public IReadOnlyList<ResourceId> AlleleIds { get; }

    public IReadOnlyList<PolicyReference> PolicyReferences { get; }

    public string DisplayName { get; }
}
