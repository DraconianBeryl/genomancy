using Genomancy.Core.Definitions;

namespace Genomancy.Core.Templates;

public sealed record GeneTemplate
{
    public GeneTemplate(ResourceId geneId, int alleleCount, IEnumerable<AlleleFrequency> alleleFrequencies)
    {
        ArgumentNullException.ThrowIfNull(alleleFrequencies);

        if (alleleCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(alleleCount), alleleCount, "Allele count must be greater than zero.");
        }

        GeneId = geneId;
        AlleleCount = alleleCount;
        AlleleFrequencies = alleleFrequencies
            .OrderBy(frequency => frequency.AlleleId)
            .ToArray();

        if (AlleleFrequencies.Count == 0)
        {
            throw new ArgumentException("Gene template must contain at least one allele frequency.", nameof(alleleFrequencies));
        }
    }

    public ResourceId GeneId { get; }

    public int AlleleCount { get; }

    public IReadOnlyList<AlleleFrequency> AlleleFrequencies { get; }

    public bool Equals(GeneTemplate? other)
    {
        return other is not null
            && GeneId == other.GeneId
            && AlleleCount == other.AlleleCount
            && AlleleFrequencies.SequenceEqual(other.AlleleFrequencies);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(GeneId);
        hash.Add(AlleleCount);

        foreach (var frequency in AlleleFrequencies)
        {
            hash.Add(frequency);
        }

        return hash.ToHashCode();
    }
}
