using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Mutation;

public static class GenomeMutationService
{
    public static GenomeMutationResult Apply(
        FrozenSystemDefinition definition,
        CurrentGenomeCopy currentGenome,
        GenomeMutationPolicy policy,
        GenomeMutationRequest request)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(currentGenome);
        ArgumentNullException.ThrowIfNull(policy);
        ArgumentNullException.ThrowIfNull(request);

        var diagnostics = Validate(definition, currentGenome.CurrentState, policy, request);

        if (diagnostics.Count > 0)
        {
            return new GenomeMutationResult(GenomeMutationResultStatus.Rejected, diagnostics: diagnostics);
        }

        var state = currentGenome.CurrentState;

        foreach (var operation in request.Operations)
        {
            state = ApplyOperation(state, operation);
        }

        currentGenome.ReplaceState(state);

        if (request.ApplicationMode == MutationApplicationMode.CurrentOnly)
        {
            return new GenomeMutationResult(GenomeMutationResultStatus.AppliedToCurrent);
        }

        var committed = currentGenome.Commit(
            request.CommitVersionId ?? throw new InvalidOperationException("Commit version id was validated but unavailable."),
            string.IsNullOrWhiteSpace(request.ChangeSummary)
                ? $"{request.SourceKind}:{request.SourceId}"
                : request.ChangeSummary);

        return new GenomeMutationResult(GenomeMutationResultStatus.Committed, committed);
    }

    public static GenomeMutationResult RepairFromBase(
        CurrentGenomeCopy currentGenome,
        ResourceId groupId,
        ResourceId? geneId = null)
    {
        ArgumentNullException.ThrowIfNull(currentGenome);

        var state = currentGenome.CurrentState;
        var baseGroup = currentGenome.BaseVersion.State.Groups.FirstOrDefault(group => group.GroupId == groupId);

        if (geneId is null)
        {
            state = baseGroup is null
                ? RemoveGroup(state, groupId)
                : state.WithGroup(baseGroup);
        }
        else
        {
            if (baseGroup is null)
            {
                return new GenomeMutationResult(
                    GenomeMutationResultStatus.Rejected,
                    diagnostics:
                    [
                        Diagnostic(
                            "MUTATION_REPAIR_BASE_GROUP_MISSING",
                            $"base/groups/{groupId}",
                            $"Base genome does not contain group '{groupId}'."),
                    ]);
            }

            var baseSet = baseGroup.GeneAlleles.FirstOrDefault(set => set.GeneId == geneId.Value);

            if (baseSet is null)
            {
                return new GenomeMutationResult(
                    GenomeMutationResultStatus.Rejected,
                    diagnostics:
                    [
                        Diagnostic(
                            "MUTATION_REPAIR_BASE_GENE_MISSING",
                            $"base/groups/{groupId}/genes/{geneId}",
                            $"Base genome group '{groupId}' does not contain gene '{geneId}'."),
                    ]);
            }

            state = state.WithGeneAlleles(groupId, baseSet);
        }

        currentGenome.ReplaceState(state);
        return new GenomeMutationResult(GenomeMutationResultStatus.Repaired);
    }

    public static GenomeMutationResult RevertCurrent(CurrentGenomeCopy currentGenome)
    {
        ArgumentNullException.ThrowIfNull(currentGenome);

        currentGenome.DiscardChanges();
        return new GenomeMutationResult(GenomeMutationResultStatus.Reverted);
    }

    private static List<GenomeMutationDiagnostic> Validate(
        FrozenSystemDefinition definition,
        GenomeState state,
        GenomeMutationPolicy policy,
        GenomeMutationRequest request)
    {
        var diagnostics = new List<GenomeMutationDiagnostic>();

        if (request.SourceKind == MutationSourceKind.ExternalRequest && !policy.AllowExternalRequests)
        {
            diagnostics.Add(Diagnostic(
                "MUTATION_EXTERNAL_SOURCE_BLOCKED",
                $"sources/{request.SourceId}",
                $"External mutation source '{request.SourceId}' is not allowed by policy."));
        }

        if (request.Operations.Count == 0)
        {
            diagnostics.Add(Diagnostic("MUTATION_NO_OPERATIONS", "operations", "At least one mutation operation is required."));
        }

        if (request.ApplicationMode == MutationApplicationMode.Commit && request.CommitVersionId is null)
        {
            diagnostics.Add(Diagnostic("MUTATION_COMMIT_VERSION_REQUIRED", "commitVersionId", "Committed mutations require a new genome version id."));
        }

        foreach (var operation in request.Operations)
        {
            ValidateOperation(definition, state, policy, operation, diagnostics);
        }

        return diagnostics;
    }

    private static void ValidateOperation(
        FrozenSystemDefinition definition,
        GenomeState state,
        GenomeMutationPolicy policy,
        GenomeMutationOperation operation,
        List<GenomeMutationDiagnostic> diagnostics)
    {
        if (policy.IsProtected(operation.Target))
        {
            diagnostics.Add(Diagnostic(
                "MUTATION_TARGET_PROTECTED",
                TargetPath(operation.Target),
                "Mutation target is protected by policy."));
            return;
        }

        switch (operation.Kind)
        {
            case MutationOperationKind.ReplaceAllele:
                ValidateGeneTarget(definition, state, operation, diagnostics);

                if (operation.AlleleId is not null)
                {
                    var gene = definition.Genes.FirstOrDefault(gene => gene.Id == operation.Target.GeneId);

                    if (gene is not null && !gene.AlleleIds.Contains(operation.AlleleId.Value))
                    {
                        diagnostics.Add(Diagnostic(
                            "MUTATION_ALLELE_NOT_ALLOWED",
                            $"{TargetPath(operation.Target)}/allele/{operation.AlleleId}",
                            $"Allele '{operation.AlleleId}' is not allowed for gene '{gene.Id}'."));
                    }
                }

                break;

            case MutationOperationKind.UpdateNumericValue:
                ValidateGeneTarget(definition, state, operation, diagnostics);
                break;

            case MutationOperationKind.SetCopyCount:
                ValidateGeneExists(definition, operation, diagnostics);
                ValidateGeneStateExists(state, operation, diagnostics);
                break;

            case MutationOperationKind.AddGroup:
                if (operation.GroupState is null)
                {
                    diagnostics.Add(Diagnostic("MUTATION_GROUP_STATE_REQUIRED", TargetPath(operation.Target), "Add-group mutation requires group state."));
                }
                else if (!definition.Groups.Any(group => group.Id == operation.GroupState.GroupId))
                {
                    diagnostics.Add(Diagnostic(
                        "MUTATION_GROUP_NOT_DEFINED",
                        $"groups/{operation.GroupState.GroupId}",
                        $"Group '{operation.GroupState.GroupId}' is not defined."));
                }

                break;

            case MutationOperationKind.RemoveGroup:
                if (!state.Groups.Any(group => group.GroupId == operation.Target.GroupId))
                {
                    diagnostics.Add(Diagnostic(
                        "MUTATION_GROUP_STATE_MISSING",
                        TargetPath(operation.Target),
                        $"Genome group '{operation.Target.GroupId}' is missing."));
                }

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(operation), operation.Kind, "Unsupported mutation operation.");
        }
    }

    private static void ValidateGeneTarget(
        FrozenSystemDefinition definition,
        GenomeState state,
        GenomeMutationOperation operation,
        List<GenomeMutationDiagnostic> diagnostics)
    {
        ValidateGeneExists(definition, operation, diagnostics);
        var alleleSet = ValidateGeneStateExists(state, operation, diagnostics);

        if (alleleSet is not null && !alleleSet.Entries.Any(entry => entry.Rank == operation.Target.AlleleRank))
        {
            diagnostics.Add(Diagnostic(
                "MUTATION_ALLELE_RANK_MISSING",
                TargetPath(operation.Target),
                $"Allele rank '{operation.Target.AlleleRank}' is missing."));
        }
    }

    private static void ValidateGeneExists(
        FrozenSystemDefinition definition,
        GenomeMutationOperation operation,
        List<GenomeMutationDiagnostic> diagnostics)
    {
        if (operation.Target.GeneId is null)
        {
            diagnostics.Add(Diagnostic("MUTATION_GENE_TARGET_REQUIRED", TargetPath(operation.Target), "Mutation operation requires a gene target."));
            return;
        }

        if (!definition.Genes.Any(gene => gene.Id == operation.Target.GeneId.Value))
        {
            diagnostics.Add(Diagnostic(
                "MUTATION_GENE_NOT_DEFINED",
                TargetPath(operation.Target),
                $"Gene '{operation.Target.GeneId}' is not defined."));
        }
    }

    private static RankedAlleleSet? ValidateGeneStateExists(
        GenomeState state,
        GenomeMutationOperation operation,
        List<GenomeMutationDiagnostic> diagnostics)
    {
        if (operation.Target.GeneId is null)
        {
            return null;
        }

        var group = state.Groups.FirstOrDefault(group => group.GroupId == operation.Target.GroupId);

        if (group is null)
        {
            diagnostics.Add(Diagnostic(
                "MUTATION_GROUP_STATE_MISSING",
                $"genome/groups/{operation.Target.GroupId}",
                $"Genome group '{operation.Target.GroupId}' is missing."));
            return null;
        }

        var alleleSet = group.GeneAlleles.FirstOrDefault(set => set.GeneId == operation.Target.GeneId.Value);

        if (alleleSet is null)
        {
            diagnostics.Add(Diagnostic(
                "MUTATION_GENE_STATE_MISSING",
                TargetPath(operation.Target),
                $"Genome gene '{operation.Target.GeneId}' is missing in group '{operation.Target.GroupId}'."));
        }

        return alleleSet;
    }

    private static GenomeState ApplyOperation(GenomeState state, GenomeMutationOperation operation)
    {
        return operation.Kind switch
        {
            MutationOperationKind.ReplaceAllele => ReplaceAllele(state, operation),
            MutationOperationKind.UpdateNumericValue => UpdateNumericValue(state, operation),
            MutationOperationKind.SetCopyCount => SetCopyCount(state, operation),
            MutationOperationKind.AddGroup => state.WithGroup(operation.GroupState ?? throw new InvalidOperationException("Group state was validated but unavailable.")),
            MutationOperationKind.RemoveGroup => RemoveGroup(state, operation.Target.GroupId),
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation.Kind, "Unsupported mutation operation."),
        };
    }

    private static GenomeState ReplaceAllele(GenomeState state, GenomeMutationOperation operation)
    {
        return ReplaceEntry(
            state,
            operation,
            entry => new RankedAlleleEntry(
                operation.AlleleId ?? throw new InvalidOperationException("Allele id was validated but unavailable."),
                entry.Rank,
                entry.NumericValue));
    }

    private static GenomeState UpdateNumericValue(GenomeState state, GenomeMutationOperation operation)
    {
        return ReplaceEntry(
            state,
            operation,
            entry => new RankedAlleleEntry(entry.AlleleId, entry.Rank, operation.NumericValue));
    }

    private static GenomeState SetCopyCount(GenomeState state, GenomeMutationOperation operation)
    {
        var copyCount = operation.CopyCount ?? throw new InvalidOperationException("Copy count was validated but unavailable.");
        var group = state.Groups.Single(group => group.GroupId == operation.Target.GroupId);
        var set = group.GeneAlleles.Single(set => set.GeneId == operation.Target.GeneId);
        var entries = set.Entries
            .OrderBy(entry => entry.Rank)
            .ThenBy(entry => entry.AlleleId)
            .ToList();

        while (entries.Count < copyCount)
        {
            var source = entries[^1];
            entries.Add(new RankedAlleleEntry(source.AlleleId, entries.Count, source.NumericValue));
        }

        entries = entries
            .Take(copyCount)
            .Select((entry, rank) => new RankedAlleleEntry(entry.AlleleId, rank, entry.NumericValue))
            .ToList();

        return state.WithGeneAlleles(operation.Target.GroupId, new RankedAlleleSet(set.GeneId, entries));
    }

    private static GenomeState ReplaceEntry(
        GenomeState state,
        GenomeMutationOperation operation,
        Func<RankedAlleleEntry, RankedAlleleEntry> replace)
    {
        var group = state.Groups.Single(group => group.GroupId == operation.Target.GroupId);
        var set = group.GeneAlleles.Single(set => set.GeneId == operation.Target.GeneId);
        var entries = set.Entries
            .Select(entry => entry.Rank == operation.Target.AlleleRank ? replace(entry) : entry)
            .ToArray();

        return state.WithGeneAlleles(operation.Target.GroupId, new RankedAlleleSet(set.GeneId, entries));
    }

    private static GenomeState RemoveGroup(GenomeState state, ResourceId groupId)
    {
        return new GenomeState(state.Groups.Where(group => group.GroupId != groupId));
    }

    private static string TargetPath(GenomeMutationTarget target)
    {
        return target.GeneId is null
            ? $"genome/groups/{target.GroupId}"
            : $"genome/groups/{target.GroupId}/genes/{target.GeneId}/alleles/{target.AlleleRank}";
    }

    private static GenomeMutationDiagnostic Diagnostic(string code, string path, string message)
    {
        return new GenomeMutationDiagnostic(code, path, message);
    }
}
