using Genomancy.Core.Reproduction;

namespace Genomancy.Core.Compatibility;

public sealed record CompatibilityRule
{
    public CompatibilityRule(
        ReproductionCompatibility compatibility,
        IEnumerable<string>? requiredRoleNames = null,
        IEnumerable<HybridMorphologyContribution>? hybridMorphologyContributions = null,
        string reason = "")
    {
        Compatibility = compatibility;
        RequiredRoleNames = (requiredRoleNames ?? [])
            .Select(roleName =>
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    throw new ArgumentException("Required role names must not be empty.", nameof(requiredRoleNames));
                }

                return roleName.Trim();
            })
            .Order(StringComparer.Ordinal)
            .ToArray();
        HybridMorphologyContributions = (hybridMorphologyContributions ?? [])
            .OrderBy(value => value.BodyPlanId)
            .ToArray();
        Reason = reason;
    }

    public ReproductionCompatibility Compatibility { get; }

    public IReadOnlyList<string> RequiredRoleNames { get; }

    public IReadOnlyList<HybridMorphologyContribution> HybridMorphologyContributions { get; }

    public string Reason { get; }
}
