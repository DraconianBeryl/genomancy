using Genomancy.Core.Definitions;

namespace Genomancy.Core.Genome;

public sealed record GenomeState
{
    public GenomeState(IEnumerable<GenomeGroupState> groups)
    {
        ArgumentNullException.ThrowIfNull(groups);

        Groups = groups
            .OrderBy(group => group.GroupId)
            .ToReadOnlyList();
    }

    public IReadOnlyList<GenomeGroupState> Groups { get; }

    public GenomeState WithGroup(GenomeGroupState group)
    {
        ArgumentNullException.ThrowIfNull(group);

        return new GenomeState(
            Groups
                .Where(existing => existing.GroupId != group.GroupId)
                .Append(group));
    }

    public GenomeState WithGeneAlleles(ResourceId groupId, RankedAlleleSet alleleSet)
    {
        ArgumentNullException.ThrowIfNull(alleleSet);

        var group = Groups.FirstOrDefault(existing => existing.GroupId == groupId)
            ?? new GenomeGroupState(groupId, []);

        return WithGroup(group.WithGeneAlleles(alleleSet));
    }

    public bool Equals(GenomeState? other)
    {
        return other is not null && Groups.SequenceEqual(other.Groups);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var group in Groups)
        {
            hash.Add(group);
        }

        return hash.ToHashCode();
    }
}
