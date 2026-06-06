using Genomancy.Core.Definitions;

namespace Genomancy.Core.Variants;

public sealed record RuntimeBodyPlanVariant
{
    public RuntimeBodyPlanVariant(
        BodyPlanVariantId id,
        SystemDefinitionVersion systemDefinitionVersion,
        ResourceId baseBodyPlanId,
        IEnumerable<ResourceId>? requiredGroupIds = null,
        IEnumerable<ResourceId>? optionalGroupIds = null,
        IEnumerable<ResourceId>? sharedGroupIds = null,
        string changeSummary = "")
    {
        Id = id;
        SystemDefinitionVersion = systemDefinitionVersion;
        BaseBodyPlanId = baseBodyPlanId;
        RequiredGroupIds = (requiredGroupIds ?? []).Order().ToArray();
        OptionalGroupIds = (optionalGroupIds ?? []).Order().ToArray();
        SharedGroupIds = (sharedGroupIds ?? []).Order().ToArray();
        ChangeSummary = changeSummary;
    }

    public BodyPlanVariantId Id { get; }

    public SystemDefinitionVersion SystemDefinitionVersion { get; }

    public ResourceId BaseBodyPlanId { get; }

    public IReadOnlyList<ResourceId> RequiredGroupIds { get; }

    public IReadOnlyList<ResourceId> OptionalGroupIds { get; }

    public IReadOnlyList<ResourceId> SharedGroupIds { get; }

    public string ChangeSummary { get; }

    public bool Equals(RuntimeBodyPlanVariant? other)
    {
        return other is not null
            && Id == other.Id
            && SystemDefinitionVersion == other.SystemDefinitionVersion
            && BaseBodyPlanId == other.BaseBodyPlanId
            && RequiredGroupIds.SequenceEqual(other.RequiredGroupIds)
            && OptionalGroupIds.SequenceEqual(other.OptionalGroupIds)
            && SharedGroupIds.SequenceEqual(other.SharedGroupIds)
            && ChangeSummary == other.ChangeSummary;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id);
        hash.Add(SystemDefinitionVersion);
        hash.Add(BaseBodyPlanId);

        foreach (var groupId in RequiredGroupIds)
        {
            hash.Add(groupId);
        }

        foreach (var groupId in OptionalGroupIds)
        {
            hash.Add(groupId);
        }

        foreach (var groupId in SharedGroupIds)
        {
            hash.Add(groupId);
        }

        hash.Add(ChangeSummary);
        return hash.ToHashCode();
    }
}
