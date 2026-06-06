using Genomancy.Core.Definitions;

namespace Genomancy.Core.Genome;

public sealed record GenomeGroupState
{
    public GenomeGroupState(ResourceId groupId, IEnumerable<RankedAlleleSet> geneAlleles)
    {
        ArgumentNullException.ThrowIfNull(geneAlleles);

        GroupId = groupId;
        GeneAlleles = geneAlleles
            .OrderBy(set => set.GeneId)
            .ToReadOnlyList();
    }

    public ResourceId GroupId { get; }

    public IReadOnlyList<RankedAlleleSet> GeneAlleles { get; }

    public GenomeGroupState WithGeneAlleles(RankedAlleleSet alleleSet)
    {
        ArgumentNullException.ThrowIfNull(alleleSet);

        return new GenomeGroupState(
            GroupId,
            GeneAlleles
                .Where(set => set.GeneId != alleleSet.GeneId)
                .Append(alleleSet));
    }

    public bool Equals(GenomeGroupState? other)
    {
        return other is not null
            && GroupId == other.GroupId
            && GeneAlleles.SequenceEqual(other.GeneAlleles);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(GroupId);

        foreach (var alleleSet in GeneAlleles)
        {
            hash.Add(alleleSet);
        }

        return hash.ToHashCode();
    }
}
