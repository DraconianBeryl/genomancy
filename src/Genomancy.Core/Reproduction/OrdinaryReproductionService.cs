using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;
using Genomancy.Core.Inheritance;
using Genomancy.Core.Randomness;

namespace Genomancy.Core.Reproduction;

public static class OrdinaryReproductionService
{
    public static ReproductionResult Reproduce(ReproductionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestDiagnostics = ValidateRequest(request);

        if (requestDiagnostics.Count > 0)
        {
            return new ReproductionResult(ReproductionResultStatus.InvalidRequest, diagnostics: requestDiagnostics);
        }

        if (request.Policy.Compatibility == ReproductionCompatibility.Sterile)
        {
            return new ReproductionResult(
                ReproductionResultStatus.Sterile,
                diagnostics: [Diagnostic("REPRODUCTION_STERILE", "policy/compatibility", "Reproduction policy reported sterile parents.")]);
        }

        if (request.Policy.Compatibility == ReproductionCompatibility.Incompatible)
        {
            return new ReproductionResult(
                ReproductionResultStatus.Incompatible,
                diagnostics: [Diagnostic("REPRODUCTION_INCOMPATIBLE", "policy/compatibility", "Reproduction policy reported incompatible parents.")]);
        }

        if (request.Policy.Compatibility == ReproductionCompatibility.Inviable)
        {
            return new ReproductionResult(
                ReproductionResultStatus.Inviable,
                diagnostics: [Diagnostic("REPRODUCTION_INVIABLE", "policy/compatibility", "Reproduction policy reported inviable offspring.")]);
        }

        var rolesByName = request.ParentRoles.ToDictionary(role => role.Name, StringComparer.Ordinal);
        var contributingRoles = ResolveContributingRoles(request, rolesByName);

        if (request.Policy.Mode == ReproductionMode.ClonalCopy)
        {
            return ReproduceClonalCopy(request, contributingRoles);
        }

        var streams = new NamedRandomStreams(request.RandomSeed);
        var groups = new List<GenomeGroupState>();
        var reproductionDiagnostics = new List<ReproductionDiagnostic>();

        foreach (var groupDefinition in request.Definition.Groups.OrderBy(group => group.Id))
        {
            var geneAlleles = new List<RankedAlleleSet>();

            foreach (var geneId in groupDefinition.GeneIds.Order())
            {
                var geneDefinition = request.Definition.Genes.First(gene => gene.Id == geneId);

                if (contributingRoles.Count != geneDefinition.RequiredAlleleCount)
                {
                    reproductionDiagnostics.Add(Diagnostic(
                        "REPRODUCTION_AMBIGUOUS_PLOIDY",
                        $"groups/{groupDefinition.Id}/genes/{geneId}/contributors",
                        $"Gene '{geneId}' requires {geneDefinition.RequiredAlleleCount} contributors but request supplies {contributingRoles.Count}."));
                    continue;
                }

                var entries = new List<RankedAlleleEntry>();

                foreach (var role in contributingRoles)
                {
                    var parentSet = role.GenomeVersion.State.Groups
                        .FirstOrDefault(group => group.GroupId == groupDefinition.Id)?
                        .GeneAlleles
                        .FirstOrDefault(set => set.GeneId == geneId);

                    if (parentSet is null)
                    {
                        reproductionDiagnostics.Add(Diagnostic(
                            "REPRODUCTION_PARENT_GENE_MISSING",
                            $"parents/{role.Name}/groups/{groupDefinition.Id}/genes/{geneId}",
                            $"Parent role '{role.Name}' is missing gene '{geneId}' in group '{groupDefinition.Id}'."));
                        continue;
                    }

                    var selected = SelectAlleleEntry(
                        request.Policy,
                        streams.Get($"allele:{groupDefinition.Id}:{geneId}:{role.Name}"),
                        role.Name,
                        groupDefinition.Id,
                        geneId,
                        parentSet);

                    if (selected is null)
                    {
                        reproductionDiagnostics.Add(Diagnostic(
                            "REPRODUCTION_NO_TRANSMISSIBLE_ALLELE",
                            $"parents/{role.Name}/groups/{groupDefinition.Id}/genes/{geneId}/entries",
                            $"Parent role '{role.Name}' has no transmissible allele for gene '{geneId}'."));
                        continue;
                    }

                    entries.Add(new RankedAlleleEntry(selected.AlleleId, entries.Count, selected.NumericValue));
                }

                if (entries.Count == geneDefinition.RequiredAlleleCount)
                {
                    geneAlleles.Add(new RankedAlleleSet(geneId, entries));
                }
            }

            if (geneAlleles.Count > 0)
            {
                groups.Add(new GenomeGroupState(groupDefinition.Id, geneAlleles));
            }
        }

        if (reproductionDiagnostics.Count > 0)
        {
            return new ReproductionResult(ReproductionResultStatus.InvalidRequest, diagnostics: reproductionDiagnostics);
        }

        var offspring = new GenomeVersion(
            request.OffspringVersionId,
            request.Definition.Version,
            request.OffspringIndividualId,
            new GenomeState(groups),
            changeSummary: string.Join(
                ";",
                contributingRoles.Select(role => $"{role.Name}={role.GenomeVersion.Id}")),
            heritableObjects: request.Policy.InheritNonPloidalObjects
                ? NonPloidalInheritanceService.Inherit(
                    contributingRoles,
                    request.RandomSeed,
                    request.Policy.IncludeInactiveNonPloidalObjects,
                    request.Policy.InheritedTraceDegradationSteps).State
                : new HeritableObjectState());

        return new ReproductionResult(ReproductionResultStatus.Success, offspring);
    }

    private static ReproductionResult ReproduceClonalCopy(
        ReproductionRequest request,
        IReadOnlyList<ReproductionParentRole> contributingRoles)
    {
        if (contributingRoles.Count != 1)
        {
            return new ReproductionResult(
                ReproductionResultStatus.InvalidRequest,
                diagnostics:
                [
                    Diagnostic(
                        "REPRODUCTION_CLONE_REQUIRES_ONE_CONTRIBUTOR",
                        "policy/contributingRoles",
                        "Clonal copy reproduction requires exactly one contributing role."),
                ]);
        }

        var source = contributingRoles[0];
        var offspring = new GenomeVersion(
            request.OffspringVersionId,
            request.Definition.Version,
            request.OffspringIndividualId,
            source.GenomeVersion.State,
            changeSummary: $"clone={source.Name}:{source.GenomeVersion.Id}",
            heritableObjects: request.Policy.InheritNonPloidalObjects
                ? source.GenomeVersion.HeritableObjects
                : new HeritableObjectState());

        return new ReproductionResult(ReproductionResultStatus.Success, offspring);
    }

    private static List<ReproductionDiagnostic> ValidateRequest(ReproductionRequest request)
    {
        var diagnostics = new List<ReproductionDiagnostic>();

        if (request.ParentRoles.Count == 0)
        {
            diagnostics.Add(Diagnostic("REPRODUCTION_NO_PARENTS", "parents", "At least one parent role is required."));
        }

        foreach (var duplicateName in request.ParentRoles.GroupBy(role => role.Name, StringComparer.Ordinal).Where(group => group.Count() > 1).Select(group => group.Key).Order(StringComparer.Ordinal))
        {
            diagnostics.Add(Diagnostic("REPRODUCTION_DUPLICATE_ROLE", $"parents/{duplicateName}", $"Parent role '{duplicateName}' appears more than once."));
        }

        foreach (var role in request.ParentRoles.OrderBy(role => role.Name, StringComparer.Ordinal))
        {
            if (role.GenomeVersion.SystemDefinitionVersion != request.Definition.Version)
            {
                diagnostics.Add(Diagnostic(
                    "REPRODUCTION_PARENT_VERSION_MISMATCH",
                    $"parents/{role.Name}/systemDefinitionVersion",
                    $"Parent role '{role.Name}' uses system definition version '{role.GenomeVersion.SystemDefinitionVersion}' instead of '{request.Definition.Version}'."));
            }
        }

        foreach (var roleName in request.Policy.ContributingRoleNames.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal))
        {
            if (!request.ParentRoles.Any(role => role.Name == roleName))
            {
                diagnostics.Add(Diagnostic(
                    "REPRODUCTION_UNKNOWN_CONTRIBUTOR",
                    $"policy/contributingRoles/{roleName}",
                    $"Contributing role '{roleName}' is not present in parent roles."));
            }
        }

        foreach (var duplicateRoleName in request.Policy.ContributingRoleNames.GroupBy(roleName => roleName, StringComparer.Ordinal).Where(group => group.Count() > 1).Select(group => group.Key).Order(StringComparer.Ordinal))
        {
            diagnostics.Add(Diagnostic(
                "REPRODUCTION_DUPLICATE_CONTRIBUTOR",
                $"policy/contributingRoles/{duplicateRoleName}",
                $"Contributing role '{duplicateRoleName}' appears more than once."));
        }

        return diagnostics;
    }

    private static IReadOnlyList<ReproductionParentRole> ResolveContributingRoles(
        ReproductionRequest request,
        IReadOnlyDictionary<string, ReproductionParentRole> rolesByName)
    {
        if (request.Policy.ContributingRoleNames.Count == 0)
        {
            return request.ParentRoles;
        }

        return request.Policy.ContributingRoleNames
            .Select(roleName => rolesByName[roleName])
            .ToArray();
    }

    private static RankedAlleleEntry? SelectAlleleEntry(
        ReproductionPolicy policy,
        DeterministicRandomStream stream,
        string roleName,
        ResourceId groupId,
        ResourceId geneId,
        RankedAlleleSet parentSet)
    {
        var weightedEntries = parentSet.Entries
            .Select(entry => new
            {
                Entry = entry,
                Weight = FindWeight(policy, roleName, groupId, geneId, entry.AlleleId),
            })
            .Where(entry => entry.Weight > 0)
            .OrderBy(entry => entry.Entry.Rank)
            .ThenBy(entry => entry.Entry.AlleleId)
            .ToArray();

        if (weightedEntries.Length == 0)
        {
            return null;
        }

        var totalWeight = weightedEntries.Sum(entry => entry.Weight);

        if (weightedEntries.Any(entry => double.IsNaN(entry.Weight) || double.IsInfinity(entry.Weight)) || totalWeight <= 0 || double.IsInfinity(totalWeight))
        {
            return null;
        }

        if (weightedEntries.All(entry => entry.Weight == 1))
        {
            return weightedEntries[stream.NextInt32(weightedEntries.Length)].Entry;
        }

        var threshold = stream.NextUnitDouble() * totalWeight;
        var running = 0.0;

        foreach (var weightedEntry in weightedEntries)
        {
            running += weightedEntry.Weight;

            if (threshold < running)
            {
                return weightedEntry.Entry;
            }
        }

        return weightedEntries[^1].Entry;
    }

    private static double FindWeight(
        ReproductionPolicy policy,
        string roleName,
        ResourceId groupId,
        ResourceId geneId,
        ResourceId alleleId)
    {
        return policy.TransmissionWeights
            .LastOrDefault(weight =>
                weight.RoleName == roleName
                && weight.GroupId == groupId
                && weight.GeneId == geneId
                && weight.AlleleId == alleleId)
            ?.Weight ?? 1;
    }

    private static ReproductionDiagnostic Diagnostic(string code, string path, string message)
    {
        return new ReproductionDiagnostic(code, path, message);
    }
}
