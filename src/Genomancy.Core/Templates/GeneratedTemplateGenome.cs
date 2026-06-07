using Genomancy.Core.Genome;

namespace Genomancy.Core.Templates;

public sealed record GeneratedTemplateGenome
{
    public GeneratedTemplateGenome(
        GenomeVersion genomeVersion,
        IEnumerable<PopulationTemplateGroupId> groupPath,
        PopulationTemplateId primaryTemplateId,
        PopulationTemplateVersionId primaryTemplateVersionId,
        PopulationTemplateId? secondaryTemplateId = null,
        PopulationTemplateVersionId? secondaryTemplateVersionId = null)
    {
        ArgumentNullException.ThrowIfNull(genomeVersion);
        ArgumentNullException.ThrowIfNull(groupPath);

        GenomeVersion = genomeVersion;
        GroupPath = Array.AsReadOnly(groupPath.ToArray());
        PrimaryTemplateId = primaryTemplateId;
        PrimaryTemplateVersionId = primaryTemplateVersionId;
        SecondaryTemplateId = secondaryTemplateId;
        SecondaryTemplateVersionId = secondaryTemplateVersionId;
    }

    public GenomeVersion GenomeVersion { get; }

    public IReadOnlyList<PopulationTemplateGroupId> GroupPath { get; }

    public PopulationTemplateId PrimaryTemplateId { get; }

    public PopulationTemplateVersionId PrimaryTemplateVersionId { get; }

    public PopulationTemplateId? SecondaryTemplateId { get; }

    public PopulationTemplateVersionId? SecondaryTemplateVersionId { get; }

    public bool WasBlended => SecondaryTemplateId is not null;

    public bool Equals(GeneratedTemplateGenome? other)
    {
        return other is not null
            && GenomeVersion == other.GenomeVersion
            && GroupPath.SequenceEqual(other.GroupPath)
            && PrimaryTemplateId == other.PrimaryTemplateId
            && PrimaryTemplateVersionId == other.PrimaryTemplateVersionId
            && SecondaryTemplateId == other.SecondaryTemplateId
            && SecondaryTemplateVersionId == other.SecondaryTemplateVersionId;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(GenomeVersion);

        foreach (var group in GroupPath)
        {
            hash.Add(group);
        }

        hash.Add(PrimaryTemplateId);
        hash.Add(PrimaryTemplateVersionId);
        hash.Add(SecondaryTemplateId);
        hash.Add(SecondaryTemplateVersionId);
        return hash.ToHashCode();
    }
}
