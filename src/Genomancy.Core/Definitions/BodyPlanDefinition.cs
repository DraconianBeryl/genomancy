namespace Genomancy.Core.Definitions;

public sealed record BodyPlanDefinition
{
    public BodyPlanDefinition(
        ResourceId id,
        IEnumerable<ResourceId> requiredGroupIds,
        IEnumerable<ResourceId>? optionalGroupIds = null,
        IEnumerable<ResourceId>? sharedGroupIds = null,
        IEnumerable<PolicyReference>? policyReferences = null,
        string displayName = "")
    {
        Id = id;
        RequiredGroupIds = requiredGroupIds.ToReadOnlyList();
        OptionalGroupIds = (optionalGroupIds ?? []).ToReadOnlyList();
        SharedGroupIds = (sharedGroupIds ?? []).ToReadOnlyList();
        PolicyReferences = (policyReferences ?? []).ToReadOnlyList();
        DisplayName = displayName;
    }

    public ResourceId Id { get; }

    public IReadOnlyList<ResourceId> RequiredGroupIds { get; }

    public IReadOnlyList<ResourceId> OptionalGroupIds { get; }

    public IReadOnlyList<ResourceId> SharedGroupIds { get; }

    public IReadOnlyList<PolicyReference> PolicyReferences { get; }

    public string DisplayName { get; }
}
