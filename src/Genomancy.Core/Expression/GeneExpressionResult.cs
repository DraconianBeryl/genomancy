using Genomancy.Core.Definitions;

namespace Genomancy.Core.Expression;

public sealed record GeneExpressionResult
{
    public GeneExpressionResult(
        ResourceId geneId,
        GeneExpressionStrategy strategy,
        IEnumerable<ResourceId> expressedAlleleIds,
        double? numericValue = null)
    {
        GeneId = geneId;
        Strategy = strategy;
        ExpressedAlleleIds = expressedAlleleIds
            .Order()
            .ToReadOnlyList();
        NumericValue = numericValue;
    }

    public ResourceId GeneId { get; }

    public GeneExpressionStrategy Strategy { get; }

    public IReadOnlyList<ResourceId> ExpressedAlleleIds { get; }

    public double? NumericValue { get; }

    public bool Equals(GeneExpressionResult? other)
    {
        return other is not null
            && GeneId == other.GeneId
            && Strategy == other.Strategy
            && ExpressedAlleleIds.SequenceEqual(other.ExpressedAlleleIds)
            && NumericValue == other.NumericValue;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(GeneId);
        hash.Add(Strategy);

        foreach (var alleleId in ExpressedAlleleIds)
        {
            hash.Add(alleleId);
        }

        hash.Add(NumericValue);
        return hash.ToHashCode();
    }
}
