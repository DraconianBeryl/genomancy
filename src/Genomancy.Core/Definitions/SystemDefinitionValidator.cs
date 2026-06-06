namespace Genomancy.Core.Definitions;

public static class SystemDefinitionValidator
{
    public static ValidationResult Validate(SystemDefinitionBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return Validate(
            builder.Alleles,
            builder.Genes,
            builder.Groups,
            builder.BodyPlans);
    }

    public static ValidationResult Validate(FrozenSystemDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        return Validate(
            definition.Alleles,
            definition.Genes,
            definition.Groups,
            definition.BodyPlans);
    }

    private static ValidationResult Validate(
        IReadOnlyList<AlleleDefinition> alleles,
        IReadOnlyList<GeneDefinition> genes,
        IReadOnlyList<GroupDefinition> groups,
        IReadOnlyList<BodyPlanDefinition> bodyPlans)
    {
        var diagnostics = new List<ValidationDiagnostic>();
        var alleleIds = CollectIds(alleles, "alleles", diagnostics);
        var geneIds = CollectIds(genes, "genes", diagnostics);
        var groupIds = CollectIds(groups, "groups", diagnostics);

        _ = CollectIds(bodyPlans, "bodyPlans", diagnostics);

        foreach (var gene in genes.OrderBy(gene => gene.Id))
        {
            AddMissingReferences(diagnostics, gene.AlleleIds, alleleIds, $"genes/{gene.Id}/alleles", gene.Id, "GENE_UNKNOWN_ALLELE");
        }

        foreach (var group in groups.OrderBy(group => group.Id))
        {
            AddMissingReferences(diagnostics, group.GeneIds, geneIds, $"groups/{group.Id}/genes", group.Id, "GROUP_UNKNOWN_GENE");
            AddMissingReferences(diagnostics, group.SubgroupIds, groupIds, $"groups/{group.Id}/subgroups", group.Id, "GROUP_UNKNOWN_SUBGROUP");
            AddMissingReferences(diagnostics, group.DependencyGroupIds, groupIds, $"groups/{group.Id}/dependencies", group.Id, "GROUP_UNKNOWN_DEPENDENCY");
        }

        foreach (var bodyPlan in bodyPlans.OrderBy(bodyPlan => bodyPlan.Id))
        {
            AddMissingReferences(diagnostics, bodyPlan.RequiredGroupIds, groupIds, $"bodyPlans/{bodyPlan.Id}/requiredGroups", bodyPlan.Id, "BODY_PLAN_UNKNOWN_REQUIRED_GROUP");
            AddMissingReferences(diagnostics, bodyPlan.OptionalGroupIds, groupIds, $"bodyPlans/{bodyPlan.Id}/optionalGroups", bodyPlan.Id, "BODY_PLAN_UNKNOWN_OPTIONAL_GROUP");
            AddMissingReferences(diagnostics, bodyPlan.SharedGroupIds, groupIds, $"bodyPlans/{bodyPlan.Id}/sharedGroups", bodyPlan.Id, "BODY_PLAN_UNKNOWN_SHARED_GROUP");
        }

        AddGroupDependencyCycles(diagnostics, groups);

        return new ValidationResult(diagnostics);
    }

    private static HashSet<ResourceId> CollectIds<T>(
        IReadOnlyList<T> definitions,
        string path,
        List<ValidationDiagnostic> diagnostics)
        where T : class
    {
        var ids = new HashSet<ResourceId>();
        var duplicateIds = definitions
            .Select(GetId)
            .GroupBy(id => id)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .Order()
            .ToArray();

        foreach (var id in duplicateIds)
        {
            diagnostics.Add(new ValidationDiagnostic(
                ValidationSeverity.Error,
                "DUPLICATE_ID",
                $"{path}/{id}",
                id,
                $"Duplicate id '{id}'."));
        }

        foreach (var definition in definitions)
        {
            ids.Add(GetId(definition));
        }

        return ids;
    }

    private static ResourceId GetId<T>(T definition)
        where T : class
    {
        return definition switch
        {
            AlleleDefinition allele => allele.Id,
            GeneDefinition gene => gene.Id,
            GroupDefinition group => group.Id,
            BodyPlanDefinition bodyPlan => bodyPlan.Id,
            _ => throw new ArgumentOutOfRangeException(nameof(definition), definition, "Unsupported definition type."),
        };
    }

    private static void AddMissingReferences(
        List<ValidationDiagnostic> diagnostics,
        IReadOnlyList<ResourceId> references,
        HashSet<ResourceId> validIds,
        string path,
        ResourceId ownerId,
        string code)
    {
        foreach (var missingId in references.Where(reference => !validIds.Contains(reference)).Order())
        {
            diagnostics.Add(new ValidationDiagnostic(
                ValidationSeverity.Error,
                code,
                $"{path}/{missingId}",
                ownerId,
                $"'{ownerId}' references missing resource '{missingId}'."));
        }
    }

    private static void AddGroupDependencyCycles(
        List<ValidationDiagnostic> diagnostics,
        IReadOnlyList<GroupDefinition> groups)
    {
        var groupsById = groups
            .GroupBy(group => group.Id)
            .Where(group => group.Count() == 1)
            .ToDictionary(group => group.Key, group => group.Single());

        foreach (var group in groupsById.Values.OrderBy(group => group.Id))
        {
            var visiting = new HashSet<ResourceId>();
            var visited = new HashSet<ResourceId>();

            if (HasCycle(group.Id, groupsById, visiting, visited))
            {
                diagnostics.Add(new ValidationDiagnostic(
                    ValidationSeverity.Error,
                    "GROUP_DEPENDENCY_CYCLE",
                    $"groups/{group.Id}/dependencies",
                    group.Id,
                    $"Group dependency cycle reaches '{group.Id}'."));
            }
        }
    }

    private static bool HasCycle(
        ResourceId id,
        IReadOnlyDictionary<ResourceId, GroupDefinition> groupsById,
        HashSet<ResourceId> visiting,
        HashSet<ResourceId> visited)
    {
        if (!groupsById.TryGetValue(id, out var group))
        {
            return false;
        }

        if (visiting.Contains(id))
        {
            return true;
        }

        if (visited.Contains(id))
        {
            return false;
        }

        visiting.Add(id);

        foreach (var dependencyId in group.DependencyGroupIds)
        {
            if (HasCycle(dependencyId, groupsById, visiting, visited))
            {
                return true;
            }
        }

        visiting.Remove(id);
        visited.Add(id);
        return false;
    }
}
