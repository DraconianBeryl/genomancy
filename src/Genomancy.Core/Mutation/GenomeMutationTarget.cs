using Genomancy.Core.Definitions;

namespace Genomancy.Core.Mutation;

public sealed record GenomeMutationTarget
{
    public GenomeMutationTarget(ResourceId groupId, ResourceId? geneId = null, int alleleRank = 0)
    {
        if (alleleRank < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(alleleRank), alleleRank, "Allele rank must be zero or greater.");
        }

        GroupId = groupId;
        GeneId = geneId;
        AlleleRank = alleleRank;
    }

    public ResourceId GroupId { get; }

    public ResourceId? GeneId { get; }

    public int AlleleRank { get; }
}
