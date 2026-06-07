using Genomancy.Core.Definitions;

namespace Genomancy.Core.Templates;

public sealed record PopulationTemplateGroupVersion
{
    public PopulationTemplateGroupVersion(
        PopulationTemplateGroupId id,
        PopulationTemplateGroupVersionId versionId,
        SystemDefinitionVersion systemDefinitionVersion,
        IEnumerable<WeightedPopulationTemplate> templates,
        IEnumerable<WeightedPopulationTemplateGroup>? childGroups = null,
        CrossTemplateBlendPolicy? crossTemplateBlendPolicy = null,
        string changeSummary = "")
    {
        ArgumentNullException.ThrowIfNull(templates);

        Id = id;
        VersionId = versionId;
        SystemDefinitionVersion = systemDefinitionVersion;
        Templates = Array.AsReadOnly(templates
            .OrderBy(entry => entry.Template.Id)
            .ThenBy(entry => entry.Template.VersionId)
            .ToArray());
        ChildGroups = Array.AsReadOnly((childGroups ?? [])
            .OrderBy(entry => entry.Group.Id)
            .ThenBy(entry => entry.Group.VersionId)
            .ToArray());
        CrossTemplateBlendPolicy = crossTemplateBlendPolicy ?? new CrossTemplateBlendPolicy(0, 0.5);
        ChangeSummary = changeSummary;

        if (Templates.Count == 0 && ChildGroups.Count == 0)
        {
            throw new ArgumentException("Template group must contain at least one direct template or child group.", nameof(templates));
        }

        foreach (var template in Templates)
        {
            if (template.Template.SystemDefinitionVersion != systemDefinitionVersion)
            {
                throw new ArgumentException("All templates in a template group must target the group's system definition version.", nameof(templates));
            }
        }

        foreach (var group in ChildGroups)
        {
            if (group.Group.SystemDefinitionVersion != systemDefinitionVersion)
            {
                throw new ArgumentException("All child template groups must target the group's system definition version.", nameof(childGroups));
            }
        }
    }

    public PopulationTemplateGroupId Id { get; }

    public PopulationTemplateGroupVersionId VersionId { get; }

    public SystemDefinitionVersion SystemDefinitionVersion { get; }

    public IReadOnlyList<WeightedPopulationTemplate> Templates { get; }

    public IReadOnlyList<WeightedPopulationTemplateGroup> ChildGroups { get; }

    public CrossTemplateBlendPolicy CrossTemplateBlendPolicy { get; }

    public string ChangeSummary { get; }

    public bool Equals(PopulationTemplateGroupVersion? other)
    {
        return other is not null
            && Id == other.Id
            && VersionId == other.VersionId
            && SystemDefinitionVersion == other.SystemDefinitionVersion
            && Templates.SequenceEqual(other.Templates)
            && ChildGroups.SequenceEqual(other.ChildGroups)
            && CrossTemplateBlendPolicy == other.CrossTemplateBlendPolicy
            && ChangeSummary == other.ChangeSummary;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id);
        hash.Add(VersionId);
        hash.Add(SystemDefinitionVersion);

        foreach (var template in Templates)
        {
            hash.Add(template);
        }

        foreach (var group in ChildGroups)
        {
            hash.Add(group);
        }

        hash.Add(CrossTemplateBlendPolicy);
        hash.Add(ChangeSummary);
        return hash.ToHashCode();
    }
}
