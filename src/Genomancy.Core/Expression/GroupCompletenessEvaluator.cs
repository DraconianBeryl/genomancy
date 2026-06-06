using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Expression;

public static class GroupCompletenessEvaluator
{
    public static GroupCompletenessResult Evaluate(
        FrozenSystemDefinition definition,
        GenomeState genomeState,
        ResourceId groupId)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(genomeState);

        var groupsById = definition.Groups.ToDictionary(group => group.Id);
        var genesById = definition.Genes.ToDictionary(gene => gene.Id);
        return Evaluate(definition, genomeState, groupId, groupsById, genesById, []);
    }

    private static GroupCompletenessResult Evaluate(
        FrozenSystemDefinition definition,
        GenomeState genomeState,
        ResourceId groupId,
        IReadOnlyDictionary<ResourceId, GroupDefinition> groupsById,
        IReadOnlyDictionary<ResourceId, GeneDefinition> genesById,
        HashSet<ResourceId> visited)
    {
        if (!groupsById.TryGetValue(groupId, out var groupDefinition))
        {
            return new GroupCompletenessResult(
                groupId,
                GroupCompletenessStatus.Missing,
                [Diagnostic("GROUP_DEFINITION_MISSING", $"groups/{groupId}", groupId, $"Group definition '{groupId}' is missing.")]);
        }

        var groupState = genomeState.Groups.FirstOrDefault(group => group.GroupId == groupId);

        if (groupState is null)
        {
            return new GroupCompletenessResult(
                groupId,
                GroupCompletenessStatus.Missing,
                [Diagnostic("GROUP_STATE_MISSING", $"genome/groups/{groupId}", groupId, $"Genome group state '{groupId}' is missing.")]);
        }

        var diagnostics = new List<ExpressionDiagnostic>();

        foreach (var dependencyId in groupDefinition.DependencyGroupIds.Order())
        {
            if (!visited.Add(dependencyId))
            {
                continue;
            }

            var dependency = Evaluate(definition, genomeState, dependencyId, groupsById, genesById, visited);

            if (!dependency.IsComplete)
            {
                diagnostics.Add(Diagnostic(
                    "GROUP_DEPENDENCY_FAILED",
                    $"genome/groups/{groupId}/dependencies/{dependencyId}",
                    groupId,
                    $"Group '{groupId}' depends on incomplete group '{dependencyId}'."));
                diagnostics.AddRange(dependency.Diagnostics);
            }
        }

        foreach (var geneId in groupDefinition.GeneIds.Order())
        {
            if (!genesById.TryGetValue(geneId, out var geneDefinition))
            {
                continue;
            }

            var alleleSet = groupState.GeneAlleles.FirstOrDefault(set => set.GeneId == geneId);

            if (alleleSet is null)
            {
                diagnostics.Add(Diagnostic(
                    "GROUP_REQUIRED_GENE_MISSING",
                    $"genome/groups/{groupId}/genes/{geneId}",
                    geneId,
                    $"Genome group '{groupId}' is missing required gene '{geneId}'."));
                continue;
            }

            if (alleleSet.Entries.Count != geneDefinition.RequiredAlleleCount)
            {
                diagnostics.Add(Diagnostic(
                    "GROUP_GENE_WRONG_PLOIDY",
                    $"genome/groups/{groupId}/genes/{geneId}/entries",
                    geneId,
                    $"Gene '{geneId}' requires {geneDefinition.RequiredAlleleCount} allele entries but found {alleleSet.Entries.Count}."));
            }
        }

        return new GroupCompletenessResult(groupId, StatusFromDiagnostics(diagnostics), diagnostics);
    }

    private static GroupCompletenessStatus StatusFromDiagnostics(IReadOnlyList<ExpressionDiagnostic> diagnostics)
    {
        if (diagnostics.Count == 0)
        {
            return GroupCompletenessStatus.Complete;
        }

        if (diagnostics.Any(diagnostic => diagnostic.Code == "GROUP_DEPENDENCY_FAILED"))
        {
            return GroupCompletenessStatus.DependencyFailed;
        }

        if (diagnostics.Any(diagnostic => diagnostic.Code == "GROUP_GENE_WRONG_PLOIDY"))
        {
            return GroupCompletenessStatus.WrongPloidy;
        }

        return GroupCompletenessStatus.Incomplete;
    }

    private static ExpressionDiagnostic Diagnostic(string code, string path, ResourceId resourceId, string message)
    {
        return new ExpressionDiagnostic(code, path, resourceId, message);
    }
}
