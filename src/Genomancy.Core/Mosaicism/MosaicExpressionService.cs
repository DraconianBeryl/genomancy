using Genomancy.Core.Definitions;
using Genomancy.Core.Expression;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Mosaicism;

public static class MosaicExpressionService
{
    public static MosaicExpressionResult EvaluateGeneInRegion(
        MosaicGenomeState mosaicState,
        FrozenSystemDefinition definition,
        MosaicRegionId regionId,
        ResourceId groupId,
        ResourceId geneId,
        ResourceId bodyPlanId,
        DevelopmentalPhaseId developmentalPhaseId,
        ExpressionExternalContext externalContext)
    {
        ArgumentNullException.ThrowIfNull(mosaicState);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(externalContext);

        var genomeVersion = mosaicState.ResolveGenomeForRegion(regionId);
        var gene = definition.Genes.FirstOrDefault(value => value.Id == geneId)
            ?? throw new ArgumentException($"Gene '{geneId}' is not defined.", nameof(geneId));
        var group = genomeVersion.State.Groups.FirstOrDefault(value => value.GroupId == groupId)
            ?? throw new ArgumentException($"Group '{groupId}' is missing from region '{regionId}'.", nameof(groupId));
        var alleleSet = group.GeneAlleles.FirstOrDefault(value => value.GeneId == geneId)
            ?? throw new ArgumentException($"Gene '{geneId}' is missing from group '{groupId}' in region '{regionId}'.", nameof(geneId));
        var context = new ExpressionEvaluationContext(
            bodyPlanId,
            developmentalPhaseId,
            group,
            externalContext);

        return new MosaicExpressionResult(
            regionId,
            genomeVersion,
            GeneExpressionEvaluator.Evaluate(context, gene, alleleSet));
    }
}
