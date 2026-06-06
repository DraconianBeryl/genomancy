namespace Genomancy.Core.Definitions;

public sealed record GroupDefinition
{
    public GroupDefinition(
        ResourceId id,
        IEnumerable<ResourceId>? geneIds = null,
        IEnumerable<ResourceId>? subgroupIds = null,
        IEnumerable<ResourceId>? dependencyGroupIds = null,
        IEnumerable<PolicyReference>? policyReferences = null,
        string displayName = "")
    {
        Id = id;
        GeneIds = (geneIds ?? []).ToReadOnlyList();
        SubgroupIds = (subgroupIds ?? []).ToReadOnlyList();
        DependencyGroupIds = (dependencyGroupIds ?? []).ToReadOnlyList();
        PolicyReferences = (policyReferences ?? []).ToReadOnlyList();
        DisplayName = displayName;
    }

    public ResourceId Id { get; }

    public IReadOnlyList<ResourceId> GeneIds { get; }

    public IReadOnlyList<ResourceId> SubgroupIds { get; }

    public IReadOnlyList<ResourceId> DependencyGroupIds { get; }

    public IReadOnlyList<PolicyReference> PolicyReferences { get; }

    public string DisplayName { get; }
}
