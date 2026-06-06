using Genomancy.Core.Reproduction;

namespace Genomancy.Core.Compatibility;

public static class CompatibilityService
{
    public static CompatibilityEvaluation Evaluate(
        IEnumerable<ReproductionParentRole> parentRoles,
        IEnumerable<CompatibilityRule> rules)
    {
        ArgumentNullException.ThrowIfNull(parentRoles);
        ArgumentNullException.ThrowIfNull(rules);

        var roleNames = parentRoles.Select(role => role.Name).ToHashSet(StringComparer.Ordinal);

        foreach (var rule in rules)
        {
            if (rule.RequiredRoleNames.All(roleNames.Contains))
            {
                return new CompatibilityEvaluation(
                    rule.Compatibility,
                    rule.HybridMorphologyContributions,
                    rule.Reason);
            }
        }

        return new CompatibilityEvaluation(ReproductionCompatibility.Fertile);
    }
}
