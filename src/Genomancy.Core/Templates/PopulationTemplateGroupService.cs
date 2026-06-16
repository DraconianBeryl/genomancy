using Genomancy.Core.Genome;
using Genomancy.Core.Randomness;

namespace Genomancy.Core.Templates;

public static class PopulationTemplateGroupService
{
    public static GeneratedTemplateGenome SampleGenome(
        PopulationTemplateGroupVersion group,
        ulong seed,
        GenomeVersionId genomeVersionId,
        ExternalIndividualId individualId)
    {
        ArgumentNullException.ThrowIfNull(group);

        return SampleGenome(group, seed, genomeVersionId, individualId, []);
    }

    public static IReadOnlyList<GeneratedTemplateGenome> GeneratePopulation(
        PopulationTemplateGroupVersion group,
        int count,
        ulong seed,
        string genomeVersionPrefix,
        string individualPrefix)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "Population count must be zero or greater.");
        }

        return Enumerable.Range(0, count)
            .Select(index => SampleGenome(
                group,
                seed + (ulong)index,
                GenomeVersionId.Parse($"{genomeVersionPrefix}{index}"),
                ExternalIndividualId.Parse($"{individualPrefix}{index}")))
            .ToArray();
    }

    public static TemplatePopulationManifest GeneratePopulationManifest(
        PopulationTemplateGroupVersion group,
        int count,
        ulong seed,
        string genomeVersionPrefix,
        string individualPrefix)
    {
        ArgumentNullException.ThrowIfNull(group);

        var generated = GeneratePopulation(group, count, seed, genomeVersionPrefix, individualPrefix);
        return CreatePopulationManifest(group, generated, seed, count, genomeVersionPrefix, individualPrefix);
    }

    public static TemplatePopulationManifest CreatePopulationManifest(
        PopulationTemplateGroupVersion group,
        IReadOnlyList<GeneratedTemplateGenome> generated,
        ulong seed,
        int requestedCount,
        string genomeVersionPrefix,
        string individualPrefix)
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(generated);

        return new TemplatePopulationManifest(
            group.Id,
            group.VersionId,
            group.SystemDefinitionVersion,
            seed,
            requestedCount,
            genomeVersionPrefix,
            individualPrefix,
            generated.Select((item, index) => new TemplatePopulationManifestEntry(
                index,
                seed + (ulong)index,
                item.GenomeVersion.Id,
                item.GenomeVersion.IndividualId,
                item.GroupPath,
                item.PrimaryTemplateId,
                item.PrimaryTemplateVersionId,
                item.SecondaryTemplateId,
                item.SecondaryTemplateVersionId)));
    }

    private static GeneratedTemplateGenome SampleGenome(
        PopulationTemplateGroupVersion group,
        ulong seed,
        GenomeVersionId genomeVersionId,
        ExternalIndividualId individualId,
        IReadOnlyList<PopulationTemplateGroupId> parentPath)
    {
        var path = parentPath.Concat([group.Id]).ToArray();
        var selected = SelectEntry(group, seed, path);

        return selected switch
        {
            WeightedPopulationTemplate template => SampleSelectedTemplate(group, template, seed, genomeVersionId, individualId, path),
            WeightedPopulationTemplateGroup childGroup => SampleGenome(childGroup.Group, seed, genomeVersionId, individualId, path),
            _ => throw new InvalidOperationException("Unsupported template-group entry selected."),
        };
    }

    private static GeneratedTemplateGenome SampleSelectedTemplate(
        PopulationTemplateGroupVersion group,
        WeightedPopulationTemplate primary,
        ulong seed,
        GenomeVersionId genomeVersionId,
        ExternalIndividualId individualId,
        IReadOnlyList<PopulationTemplateGroupId> path)
    {
        var secondary = TrySelectBlendPartner(group, primary, seed, path);

        if (secondary is null)
        {
            return new GeneratedTemplateGenome(
                PopulationTemplateService.SampleGenome(primary.Template, seed, genomeVersionId, individualId),
                path,
                primary.Template.Id,
                primary.Template.VersionId);
        }

        var blended = PopulationTemplateService.Blend(
            primary.Template,
            secondary.Template,
            group.CrossTemplateBlendPolicy.SecondTemplateWeight,
            CreateBlendTemplateId(primary.Template.Id, secondary.Template.Id),
            CreateBlendTemplateVersionId(group.VersionId, primary.Template.VersionId, secondary.Template.VersionId),
            $"templateGroupBlend={group.Id}:{group.VersionId}");

        var sampled = PopulationTemplateService.SampleGenome(blended, seed, genomeVersionId, individualId);
        var genome = new GenomeVersion(
            sampled.Id,
            sampled.SystemDefinitionVersion,
            sampled.IndividualId,
            sampled.State,
            sampled.ParentVersionId,
            $"{sampled.ChangeSummary};templateGroupBlend={group.Id}:{group.VersionId}",
            sampled.HeritableObjects);

        return new GeneratedTemplateGenome(
            genome,
            path,
            primary.Template.Id,
            primary.Template.VersionId,
            secondary.Template.Id,
            secondary.Template.VersionId);
    }

    private static object SelectEntry(PopulationTemplateGroupVersion group, ulong seed, IReadOnlyList<PopulationTemplateGroupId> path)
    {
        var entries = group.Templates
            .Cast<object>()
            .Concat(group.ChildGroups)
            .Select(entry => (Entry: entry, Weight: SelectionWeight(entry)))
            .Where(entry => entry.Weight > 0)
            .ToArray();

        if (entries.Length == 0)
        {
            throw new InvalidOperationException($"Template group '{group.Id}' has no positive-weight templates or child groups.");
        }

        return SelectWeighted(entries, Stream(seed, path, $"group:{group.Id}:entry"));
    }

    private static WeightedPopulationTemplate? TrySelectBlendPartner(
        PopulationTemplateGroupVersion group,
        WeightedPopulationTemplate primary,
        ulong seed,
        IReadOnlyList<PopulationTemplateGroupId> path)
    {
        if (group.CrossTemplateBlendPolicy.Rate <= 0)
        {
            return null;
        }

        var chance = Stream(seed, path, $"group:{group.Id}:blend:{primary.Template.Id}").NextUnitDouble();

        if (chance >= group.CrossTemplateBlendPolicy.Rate)
        {
            return null;
        }

        var candidates = group.Templates
            .Where(candidate => candidate.Weight > 0
                && (candidate.Template.Id != primary.Template.Id || candidate.Template.VersionId != primary.Template.VersionId))
            .Select(candidate => (Entry: candidate, candidate.Weight))
            .ToArray();

        if (candidates.Length == 0)
        {
            return null;
        }

        return SelectWeighted(candidates, Stream(seed, path, $"group:{group.Id}:blend-partner:{primary.Template.Id}"));
    }

    private static double SelectionWeight(object entry)
    {
        return entry switch
        {
            WeightedPopulationTemplate template => template.Weight,
            WeightedPopulationTemplateGroup group => group.Weight,
            _ => 0,
        };
    }

    private static T SelectWeighted<T>((T Entry, double Weight)[] entries, DeterministicRandomStream stream)
    {
        var totalWeight = entries.Sum(entry => entry.Weight);
        var threshold = stream.NextUnitDouble() * totalWeight;
        var running = 0.0;

        foreach (var entry in entries)
        {
            running += entry.Weight;

            if (threshold < running)
            {
                return entry.Entry;
            }
        }

        return entries[^1].Entry;
    }

    private static DeterministicRandomStream Stream(
        ulong seed,
        IReadOnlyList<PopulationTemplateGroupId> path,
        string name)
    {
        return DeterministicRandomStream.FromSeedAndName(seed, $"templateGroup:{string.Join("/", path.Select(id => id.Value))}:{name}");
    }

    private static PopulationTemplateId CreateBlendTemplateId(PopulationTemplateId first, PopulationTemplateId second)
    {
        return PopulationTemplateId.Parse($"blend:{first}:with:{second}");
    }

    private static PopulationTemplateVersionId CreateBlendTemplateVersionId(
        PopulationTemplateGroupVersionId groupVersionId,
        PopulationTemplateVersionId first,
        PopulationTemplateVersionId second)
    {
        return PopulationTemplateVersionId.Parse($"blend:{groupVersionId}:{first}:with:{second}");
    }
}
