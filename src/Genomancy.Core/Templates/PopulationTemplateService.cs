using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;
using Genomancy.Core.Randomness;

namespace Genomancy.Core.Templates;

public static class PopulationTemplateService
{
    public static GenomeVersion SampleGenome(
        PopulationTemplateVersion template,
        ulong seed,
        GenomeVersionId genomeVersionId,
        ExternalIndividualId individualId)
    {
        ArgumentNullException.ThrowIfNull(template);

        var groups = template.GroupTemplates
            .Select(group => new GenomeGroupState(
                group.GroupId,
                group.GeneTemplates.Select(gene => SampleGene(template.Id, group.GroupId, gene, seed))))
            .ToArray();

        return new GenomeVersion(
            genomeVersionId,
            template.SystemDefinitionVersion,
            individualId,
            new GenomeState(groups),
            changeSummary: $"sampledFrom={template.Id}:{template.VersionId}");
    }

    public static IReadOnlyList<GenomeVersion> GeneratePopulation(
        PopulationTemplateVersion template,
        int count,
        ulong seed,
        string genomeVersionPrefix,
        string individualPrefix)
    {
        ArgumentNullException.ThrowIfNull(template);

        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, "Population count must be zero or greater.");
        }

        return Enumerable.Range(0, count)
            .Select(index => SampleGenome(
                template,
                seed + (ulong)index,
                GenomeVersionId.Parse($"{genomeVersionPrefix}{index}"),
                ExternalIndividualId.Parse($"{individualPrefix}{index}")))
            .ToArray();
    }

    public static PopulationTemplateVersion Blend(
        PopulationTemplateVersion first,
        PopulationTemplateVersion second,
        double secondWeight,
        PopulationTemplateId id,
        PopulationTemplateVersionId versionId,
        string changeSummary = "")
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        if (first.SystemDefinitionVersion != second.SystemDefinitionVersion)
        {
            throw new ArgumentException("Blended templates must target the same system definition version.", nameof(second));
        }

        if (double.IsNaN(secondWeight) || double.IsInfinity(secondWeight) || secondWeight < 0 || secondWeight > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(secondWeight), secondWeight, "Blend weight must be between zero and one.");
        }

        var firstWeight = 1 - secondWeight;
        var firstGroups = first.GroupTemplates.ToDictionary(group => group.GroupId);
        var secondGroups = second.GroupTemplates.ToDictionary(group => group.GroupId);
        var groups = firstGroups.Keys
            .Concat(secondGroups.Keys)
            .Distinct()
            .Order()
            .Select(groupId => BlendGroup(
                groupId,
                firstGroups.TryGetValue(groupId, out var firstGroup) ? firstGroup : null,
                secondGroups.TryGetValue(groupId, out var secondGroup) ? secondGroup : null,
                firstWeight,
                secondWeight))
            .ToArray();

        return new PopulationTemplateVersion(
            id,
            versionId,
            first.SystemDefinitionVersion,
            groups,
            changeSummary);
    }

    public static PopulationTemplateVersion FromGenome(
        GenomeVersion genomeVersion,
        PopulationTemplateId id,
        PopulationTemplateVersionId versionId,
        string changeSummary = "")
    {
        ArgumentNullException.ThrowIfNull(genomeVersion);

        return new PopulationTemplateVersion(
            id,
            versionId,
            genomeVersion.SystemDefinitionVersion,
            genomeVersion.State.Groups.Select(group => new GroupTemplate(
                group.GroupId,
                group.GeneAlleles.Select(set => new GeneTemplate(
                    set.GeneId,
                    set.Entries.Count,
                    set.Entries.Select(entry => new AlleleFrequency(entry.AlleleId, 1, entry.NumericValue)))))),
            changeSummary);
    }

    private static RankedAlleleSet SampleGene(
        PopulationTemplateId templateId,
        ResourceId groupId,
        GeneTemplate gene,
        ulong seed)
    {
        var entries = Enumerable.Range(0, gene.AlleleCount)
            .Select(rank =>
            {
                var stream = DeterministicRandomStream.FromSeedAndName(seed, $"template:{templateId}:group:{groupId}:gene:{gene.GeneId}:rank:{rank}");
                var selected = SelectFrequency(gene, stream);
                return new RankedAlleleEntry(selected.AlleleId, rank, selected.NumericValue);
            })
            .ToArray();

        return new RankedAlleleSet(gene.GeneId, entries);
    }

    private static AlleleFrequency SelectFrequency(GeneTemplate gene, DeterministicRandomStream stream)
    {
        var candidates = gene.AlleleFrequencies
            .Where(frequency => frequency.Weight > 0)
            .OrderBy(frequency => frequency.AlleleId)
            .ToArray();

        if (candidates.Length == 0)
        {
            throw new InvalidOperationException($"Gene template '{gene.GeneId}' has no positive-weight alleles.");
        }

        var totalWeight = candidates.Sum(frequency => frequency.Weight);
        var threshold = stream.NextUnitDouble() * totalWeight;
        var running = 0.0;

        foreach (var candidate in candidates)
        {
            running += candidate.Weight;

            if (threshold < running)
            {
                return candidate;
            }
        }

        return candidates[^1];
    }

    private static GroupTemplate BlendGroup(
        ResourceId groupId,
        GroupTemplate? first,
        GroupTemplate? second,
        double firstWeight,
        double secondWeight)
    {
        var firstGenes = first?.GeneTemplates.ToDictionary(gene => gene.GeneId) ?? [];
        var secondGenes = second?.GeneTemplates.ToDictionary(gene => gene.GeneId) ?? [];
        var genes = firstGenes.Keys
            .Concat(secondGenes.Keys)
            .Distinct()
            .Order()
            .Select(geneId => BlendGene(
                geneId,
                firstGenes.TryGetValue(geneId, out var firstGene) ? firstGene : null,
                secondGenes.TryGetValue(geneId, out var secondGene) ? secondGene : null,
                firstWeight,
                secondWeight))
            .ToArray();

        return new GroupTemplate(groupId, genes);
    }

    private static GeneTemplate BlendGene(
        ResourceId geneId,
        GeneTemplate? first,
        GeneTemplate? second,
        double firstWeight,
        double secondWeight)
    {
        var templates = new[] { first, second }.Where(template => template is not null).ToArray();
        var alleleCount = templates.Max(template => template?.AlleleCount ?? 0);
        var weightedAlleles = new Dictionary<ResourceId, (double Weight, double? NumericValue)>();

        AddFrequencies(first, firstWeight, weightedAlleles);
        AddFrequencies(second, secondWeight, weightedAlleles);

        return new GeneTemplate(
            geneId,
            alleleCount,
            weightedAlleles
                .OrderBy(value => value.Key)
                .Select(value => new AlleleFrequency(value.Key, value.Value.Weight, value.Value.NumericValue)));
    }

    private static void AddFrequencies(
        GeneTemplate? template,
        double templateWeight,
        Dictionary<ResourceId, (double Weight, double? NumericValue)> weightedAlleles)
    {
        if (template is null)
        {
            return;
        }

        foreach (var frequency in template.AlleleFrequencies)
        {
            var existing = weightedAlleles.TryGetValue(frequency.AlleleId, out var value)
                ? value
                : (Weight: 0.0, NumericValue: frequency.NumericValue);
            weightedAlleles[frequency.AlleleId] = (existing.Weight + (frequency.Weight * templateWeight), existing.NumericValue ?? frequency.NumericValue);
        }
    }
}
