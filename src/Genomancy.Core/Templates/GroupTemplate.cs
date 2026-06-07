using Genomancy.Core.Definitions;

namespace Genomancy.Core.Templates;

public sealed record GroupTemplate
{
    public GroupTemplate(ResourceId groupId, IEnumerable<GeneTemplate> geneTemplates)
    {
        ArgumentNullException.ThrowIfNull(geneTemplates);

        GroupId = groupId;
        GeneTemplates = geneTemplates
            .OrderBy(template => template.GeneId)
            .ToArray();
    }

    public ResourceId GroupId { get; }

    public IReadOnlyList<GeneTemplate> GeneTemplates { get; }

    public bool Equals(GroupTemplate? other)
    {
        return other is not null
            && GroupId == other.GroupId
            && GeneTemplates.SequenceEqual(other.GeneTemplates);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(GroupId);

        foreach (var template in GeneTemplates)
        {
            hash.Add(template);
        }

        return hash.ToHashCode();
    }
}
