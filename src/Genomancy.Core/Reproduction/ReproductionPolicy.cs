namespace Genomancy.Core.Reproduction;

public sealed record ReproductionPolicy
{
    public ReproductionPolicy(
        ReproductionCompatibility compatibility = ReproductionCompatibility.Fertile,
        IEnumerable<string>? contributingRoleNames = null,
        IEnumerable<TransmissionWeight>? transmissionWeights = null,
        bool inheritNonPloidalObjects = false,
        bool includeInactiveNonPloidalObjects = false,
        double inheritedTraceDegradationSteps = 0)
    {
        if (double.IsNaN(inheritedTraceDegradationSteps) || double.IsInfinity(inheritedTraceDegradationSteps) || inheritedTraceDegradationSteps < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(inheritedTraceDegradationSteps),
                inheritedTraceDegradationSteps,
                "Inherited trace degradation steps must be finite and zero or greater.");
        }

        Compatibility = compatibility;
        ContributingRoleNames = (contributingRoleNames ?? [])
            .Select(roleName =>
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    throw new ArgumentException("Contributing role names must not be empty.", nameof(contributingRoleNames));
                }

                return roleName.Trim();
            })
            .ToArray();
        TransmissionWeights = (transmissionWeights ?? [])
            .OrderBy(weight => weight.RoleName, StringComparer.Ordinal)
            .ThenBy(weight => weight.GroupId)
            .ThenBy(weight => weight.GeneId)
            .ThenBy(weight => weight.AlleleId)
            .ToArray();
        InheritNonPloidalObjects = inheritNonPloidalObjects;
        IncludeInactiveNonPloidalObjects = includeInactiveNonPloidalObjects;
        InheritedTraceDegradationSteps = inheritedTraceDegradationSteps;
    }

    public ReproductionCompatibility Compatibility { get; }

    public IReadOnlyList<string> ContributingRoleNames { get; }

    public IReadOnlyList<TransmissionWeight> TransmissionWeights { get; }

    public bool InheritNonPloidalObjects { get; }

    public bool IncludeInactiveNonPloidalObjects { get; }

    public double InheritedTraceDegradationSteps { get; }
}
