using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Expression;

public static class GeneExpressionEvaluator
{
    public static GeneExpressionResult Evaluate(
        ExpressionEvaluationContext context,
        GeneDefinition gene,
        RankedAlleleSet alleleSet)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(gene);
        ArgumentNullException.ThrowIfNull(alleleSet);

        if (gene.Id != alleleSet.GeneId)
        {
            throw new ArgumentException("Allele set gene id must match the gene definition id.", nameof(alleleSet));
        }

        return gene.ExpressionStrategy switch
        {
            GeneExpressionStrategy.StrictDominance => StrictDominance(gene, alleleSet),
            GeneExpressionStrategy.Codominance => Codominance(gene, alleleSet),
            GeneExpressionStrategy.NumericMidpoint => NumericMidpoint(gene, alleleSet),
            _ => throw new ArgumentOutOfRangeException(nameof(gene), gene.ExpressionStrategy, "Unsupported expression strategy."),
        };
    }

    private static GeneExpressionResult StrictDominance(GeneDefinition gene, RankedAlleleSet alleleSet)
    {
        var expressed = alleleSet.Entries
            .OrderBy(entry => entry.Rank)
            .ThenBy(entry => entry.AlleleId)
            .First();

        return new GeneExpressionResult(
            gene.Id,
            GeneExpressionStrategy.StrictDominance,
            [expressed.AlleleId],
            expressed.NumericValue);
    }

    private static GeneExpressionResult Codominance(GeneDefinition gene, RankedAlleleSet alleleSet)
    {
        return new GeneExpressionResult(
            gene.Id,
            GeneExpressionStrategy.Codominance,
            alleleSet.Entries.Select(entry => entry.AlleleId));
    }

    private static GeneExpressionResult NumericMidpoint(GeneDefinition gene, RankedAlleleSet alleleSet)
    {
        var values = alleleSet.Entries
            .Select(entry => entry.NumericValue)
            .ToArray();

        if (values.Any(value => value is null))
        {
            throw new ArgumentException("Numeric midpoint expression requires every allele entry to carry a numeric value.", nameof(alleleSet));
        }

        return new GeneExpressionResult(
            gene.Id,
            GeneExpressionStrategy.NumericMidpoint,
            alleleSet.Entries.Select(entry => entry.AlleleId),
            values.Average(value => value ?? 0));
    }
}
