using Genomancy.Core.Definitions;

namespace Genomancy.Core.Compatibility;

public sealed record HybridMorphologyContribution
{
    public HybridMorphologyContribution(ResourceId bodyPlanId, double weight)
    {
        if (double.IsNaN(weight) || double.IsInfinity(weight) || weight < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weight), weight, "Hybrid morphology weight must be finite and zero or greater.");
        }

        BodyPlanId = bodyPlanId;
        Weight = weight;
    }

    public ResourceId BodyPlanId { get; }

    public double Weight { get; }
}
