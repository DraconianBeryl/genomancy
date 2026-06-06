using Genomancy.Core.Definitions;

namespace Genomancy.Core.Development;

public sealed record MaternalEffect
{
    public MaternalEffect(ResourceId groupId, ResourceId geneId, int alleleRank, double numericDelta)
    {
        if (alleleRank < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(alleleRank), alleleRank, "Allele rank must be zero or greater.");
        }

        if (double.IsNaN(numericDelta) || double.IsInfinity(numericDelta))
        {
            throw new ArgumentOutOfRangeException(nameof(numericDelta), numericDelta, "Numeric delta must be finite.");
        }

        GroupId = groupId;
        GeneId = geneId;
        AlleleRank = alleleRank;
        NumericDelta = numericDelta;
    }

    public ResourceId GroupId { get; }

    public ResourceId GeneId { get; }

    public int AlleleRank { get; }

    public double NumericDelta { get; }
}
