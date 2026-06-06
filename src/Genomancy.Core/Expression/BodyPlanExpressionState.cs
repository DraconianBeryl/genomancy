using Genomancy.Core.Definitions;

namespace Genomancy.Core.Expression;

public sealed record BodyPlanExpressionState
{
    public BodyPlanExpressionState(IEnumerable<ResourceId>? activeBodyPlanIds = null)
    {
        ActiveBodyPlanIds = (activeBodyPlanIds ?? [])
            .Order()
            .ToReadOnlyList();
    }

    public IReadOnlyList<ResourceId> ActiveBodyPlanIds { get; }

    public bool IsActive(ResourceId bodyPlanId) => ActiveBodyPlanIds.Contains(bodyPlanId);

    public BodyPlanExpressionState Activate(ResourceId bodyPlanId)
    {
        return new BodyPlanExpressionState(ActiveBodyPlanIds.Append(bodyPlanId).Distinct());
    }

    public BodyPlanExpressionState Deactivate(ResourceId bodyPlanId)
    {
        return new BodyPlanExpressionState(ActiveBodyPlanIds.Where(id => id != bodyPlanId));
    }
}
