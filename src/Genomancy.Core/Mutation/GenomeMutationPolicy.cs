using Genomancy.Core.Definitions;

namespace Genomancy.Core.Mutation;

public sealed record GenomeMutationPolicy
{
    public GenomeMutationPolicy(
        IEnumerable<ResourceId>? protectedGroupIds = null,
        IEnumerable<ResourceId>? protectedGeneIds = null,
        bool allowExternalRequests = true)
    {
        ProtectedGroupIds = (protectedGroupIds ?? [])
            .Order()
            .ToArray();
        ProtectedGeneIds = (protectedGeneIds ?? [])
            .Order()
            .ToArray();
        AllowExternalRequests = allowExternalRequests;
    }

    public IReadOnlyList<ResourceId> ProtectedGroupIds { get; }

    public IReadOnlyList<ResourceId> ProtectedGeneIds { get; }

    public bool AllowExternalRequests { get; }

    public bool IsProtected(GenomeMutationTarget target)
    {
        return ProtectedGroupIds.Contains(target.GroupId)
            || (target.GeneId is not null && ProtectedGeneIds.Contains(target.GeneId.Value));
    }
}
