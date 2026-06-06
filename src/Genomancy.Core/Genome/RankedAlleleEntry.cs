using Genomancy.Core.Definitions;

namespace Genomancy.Core.Genome;

public sealed record RankedAlleleEntry
{
    public RankedAlleleEntry(ResourceId alleleId, int rank, double? numericValue = null)
    {
        if (rank < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rank), rank, "Rank must be zero or greater.");
        }

        AlleleId = alleleId;
        Rank = rank;
        NumericValue = numericValue;
    }

    public ResourceId AlleleId { get; }

    public int Rank { get; }

    public double? NumericValue { get; }
}
