using Genomancy.Core.Definitions;

namespace Genomancy.Core.Templates;

public sealed record PopulationTemplateVersion
{
    public PopulationTemplateVersion(
        PopulationTemplateId id,
        PopulationTemplateVersionId versionId,
        SystemDefinitionVersion systemDefinitionVersion,
        IEnumerable<GroupTemplate> groupTemplates,
        string changeSummary = "")
    {
        ArgumentNullException.ThrowIfNull(groupTemplates);

        Id = id;
        VersionId = versionId;
        SystemDefinitionVersion = systemDefinitionVersion;
        GroupTemplates = groupTemplates
            .OrderBy(template => template.GroupId)
            .ToArray();
        ChangeSummary = changeSummary;
    }

    public PopulationTemplateId Id { get; }

    public PopulationTemplateVersionId VersionId { get; }

    public SystemDefinitionVersion SystemDefinitionVersion { get; }

    public IReadOnlyList<GroupTemplate> GroupTemplates { get; }

    public string ChangeSummary { get; }

    public bool Equals(PopulationTemplateVersion? other)
    {
        return other is not null
            && Id == other.Id
            && VersionId == other.VersionId
            && SystemDefinitionVersion == other.SystemDefinitionVersion
            && GroupTemplates.SequenceEqual(other.GroupTemplates)
            && ChangeSummary == other.ChangeSummary;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id);
        hash.Add(VersionId);
        hash.Add(SystemDefinitionVersion);

        foreach (var template in GroupTemplates)
        {
            hash.Add(template);
        }

        hash.Add(ChangeSummary);
        return hash.ToHashCode();
    }
}
