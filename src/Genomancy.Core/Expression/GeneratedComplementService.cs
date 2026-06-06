using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Expression;

public static class GeneratedComplementService
{
    public static GeneratedComplementResult ApplyMissingComplement(
        FrozenSystemDefinition definition,
        GenomeState state,
        GeneratedComplementPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(policy);

        if (!definition.Groups.Any(group => group.Id == policy.GroupId))
        {
            return new GeneratedComplementResult(
                state,
                wasGenerated: false,
                diagnostics:
                [
                    new ExpressionDiagnostic(
                        "GENERATED_COMPLEMENT_UNKNOWN_GROUP",
                        $"groups/{policy.GroupId}",
                        policy.GroupId,
                        $"Generated complement group '{policy.GroupId}' is not defined."),
                ]);
        }

        if (state.Groups.Any(group => group.GroupId == policy.GroupId))
        {
            return new GeneratedComplementResult(state, wasGenerated: false);
        }

        var genesById = definition.Genes.ToDictionary(gene => gene.Id);
        var diagnostics = new List<ExpressionDiagnostic>();

        foreach (var alleleSet in policy.GeneratedGeneAlleles)
        {
            if (!genesById.TryGetValue(alleleSet.GeneId, out var gene))
            {
                diagnostics.Add(new ExpressionDiagnostic(
                    "GENERATED_COMPLEMENT_UNKNOWN_GENE",
                    $"groups/{policy.GroupId}/genes/{alleleSet.GeneId}",
                    alleleSet.GeneId,
                    $"Generated complement gene '{alleleSet.GeneId}' is not defined."));
                continue;
            }

            foreach (var entry in alleleSet.Entries.Where(entry => !gene.AlleleIds.Contains(entry.AlleleId)))
            {
                diagnostics.Add(new ExpressionDiagnostic(
                    "GENERATED_COMPLEMENT_UNKNOWN_ALLELE",
                    $"groups/{policy.GroupId}/genes/{alleleSet.GeneId}/alleles/{entry.AlleleId}",
                    entry.AlleleId,
                    $"Generated complement allele '{entry.AlleleId}' is not allowed for gene '{alleleSet.GeneId}'."));
            }
        }

        if (diagnostics.Count > 0)
        {
            return new GeneratedComplementResult(state, wasGenerated: false, diagnostics);
        }

        return new GeneratedComplementResult(
            state.WithGroup(new GenomeGroupState(policy.GroupId, policy.GeneratedGeneAlleles)),
            wasGenerated: true);
    }
}
