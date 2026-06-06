using Genomancy.Core.Reproduction;

namespace Genomancy.Core.Compatibility;

public sealed record CompatibilityEvaluation
{
    public CompatibilityEvaluation(
        ReproductionCompatibility compatibility,
        IEnumerable<HybridMorphologyContribution>? hybridMorphologyContributions = null,
        string reason = "")
    {
        Compatibility = compatibility;
        HybridMorphologyContributions = (hybridMorphologyContributions ?? [])
            .OrderBy(value => value.BodyPlanId)
            .ToArray();
        Reason = reason;
    }

    public ReproductionCompatibility Compatibility { get; }

    public IReadOnlyList<HybridMorphologyContribution> HybridMorphologyContributions { get; }

    public string Reason { get; }
}
