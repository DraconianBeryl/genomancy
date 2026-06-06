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
            GeneExpressionStrategy.NumericSum => NumericSum(gene, alleleSet),
            GeneExpressionStrategy.NumericWeightedAverage => NumericWeightedAverage(gene, alleleSet),
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
        var values = RequiredNumericValues(alleleSet);

        return new GeneExpressionResult(
            gene.Id,
            GeneExpressionStrategy.NumericMidpoint,
            alleleSet.Entries.Select(entry => entry.AlleleId),
            values.Average(value => value ?? 0));
    }

    private static GeneExpressionResult NumericSum(GeneDefinition gene, RankedAlleleSet alleleSet)
    {
        var values = RequiredNumericValues(alleleSet);

        return new GeneExpressionResult(
            gene.Id,
            GeneExpressionStrategy.NumericSum,
            alleleSet.Entries.Select(entry => entry.AlleleId),
            values.Sum(value => value ?? 0));
    }

    private static GeneExpressionResult NumericWeightedAverage(GeneDefinition gene, RankedAlleleSet alleleSet)
    {
        var values = RequiredNumericValues(alleleSet);
        var weights = alleleSet.Entries.Select(entry => 1.0 / (entry.Rank + 1)).ToArray();
        var weightedTotal = values.Zip(weights, (value, weight) => (value ?? 0) * weight).Sum();
        var totalWeight = weights.Sum();

        return new GeneExpressionResult(
            gene.Id,
            GeneExpressionStrategy.NumericWeightedAverage,
            alleleSet.Entries.Select(entry => entry.AlleleId),
            weightedTotal / totalWeight);
    }

    private static double?[] RequiredNumericValues(RankedAlleleSet alleleSet)
    {
        var values = alleleSet.Entries
            .Select(entry => entry.NumericValue)
            .ToArray();

        if (values.Any(value => value is null))
        {
            throw new ArgumentException("Numeric expression requires every allele entry to carry a numeric value.", nameof(alleleSet));
        }

        return values;
    }
}
