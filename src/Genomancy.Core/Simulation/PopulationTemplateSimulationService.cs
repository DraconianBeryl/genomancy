using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;
using Genomancy.Core.Templates;

namespace Genomancy.Core.Simulation;

public static class PopulationTemplateSimulationService
{
    public static PopulationTemplateSimulationResult MeasureAlleleFrequency(
        PopulationTemplateVersion template,
        ResourceId groupId,
        ResourceId geneId,
        ResourceId alleleId,
        int sampleCount,
        ulong seed,
        StatisticalTolerance tolerance,
        SimulationResourceLimits? limits = null,
        string testIdentifier = "population-template-simulation")
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(tolerance);

        limits ??= new SimulationResourceLimits();
        limits.ValidateSampleCount(sampleCount);

        var matchingObservations = 0;
        var observations = 0;

        for (var index = 0; index < sampleCount; index++)
        {
            var genome = PopulationTemplateService.SampleGenome(
                template,
                seed + (ulong)index,
                GenomeVersionId.Parse($"simulation.{index}"),
                ExternalIndividualId.Parse($"simulation:{index}"));
            var entries = genome.State.Groups
                .Single(group => group.GroupId == groupId)
                .GeneAlleles
                .Single(gene => gene.GeneId == geneId)
                .Entries;

            observations += entries.Count;
            matchingObservations += entries.Count(entry => entry.AlleleId == alleleId);
        }

        var observedProportion = (double)matchingObservations / observations;
        var operationPath = $"templates/{template.Id}/groups/{groupId}/genes/{geneId}/alleles/{alleleId}";
        ReproducibilityPacket? packet = null;

        if (!tolerance.Contains(observedProportion))
        {
            var diagnostic =
                $"Observed allele proportion {observedProportion:R} was outside " +
                $"[{tolerance.Minimum:R}, {tolerance.Maximum:R}] over {observations} observations.";
            packet = new ReproducibilityPacket(
                template.SystemDefinitionVersion,
                testIdentifier,
                seed,
                operationPath,
                PopulationTemplateJsonCodec.WriteToText(template),
                $"allele proportion within [{tolerance.Minimum:R}, {tolerance.Maximum:R}]",
                tolerance.ExpectedProportion,
                observedProportion,
                diagnostic);
        }

        return new PopulationTemplateSimulationResult(
            sampleCount,
            observations,
            matchingObservations,
            observedProportion,
            tolerance,
            packet);
    }
}
