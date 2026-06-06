using Genomancy.Core.Definitions;

namespace Genomancy.Core.Genome;

public sealed record RankedAlleleSet
{
    public RankedAlleleSet(ResourceId geneId, IEnumerable<RankedAlleleEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        GeneId = geneId;
        Entries = entries
            .OrderBy(entry => entry.Rank)
            .ThenBy(entry => entry.AlleleId)
            .ToReadOnlyList();

        if (Entries.Count == 0)
        {
            throw new ArgumentException("A ranked allele set must contain at least one entry.", nameof(entries));
        }
    }

    public ResourceId GeneId { get; }

    public IReadOnlyList<RankedAlleleEntry> Entries { get; }

    public bool Equals(RankedAlleleSet? other)
    {
        return other is not null
            && GeneId == other.GeneId
            && Entries.SequenceEqual(other.Entries);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(GeneId);

        foreach (var entry in Entries)
        {
            hash.Add(entry);
        }

        return hash.ToHashCode();
    }
}
