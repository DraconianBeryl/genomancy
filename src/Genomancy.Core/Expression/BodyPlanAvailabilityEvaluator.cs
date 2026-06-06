using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Expression;

public static class BodyPlanAvailabilityEvaluator
{
    public static BodyPlanAvailabilityResult Evaluate(
        FrozenSystemDefinition definition,
        GenomeState genomeState,
        BodyPlanExpressionState expressionState,
        ResourceId bodyPlanId,
        DevelopmentalPhaseId developmentalPhaseId,
        ExpressionExternalContext externalContext)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(genomeState);
        ArgumentNullException.ThrowIfNull(expressionState);
        ArgumentNullException.ThrowIfNull(externalContext);

        var bodyPlan = definition.BodyPlans.FirstOrDefault(plan => plan.Id == bodyPlanId);

        if (bodyPlan is null)
        {
            return new BodyPlanAvailabilityResult(
                bodyPlanId,
                BodyPlanAvailabilityStatus.Unavailable,
                diagnostics:
                [
                    new ExpressionDiagnostic(
                        "BODY_PLAN_UNKNOWN",
                        $"bodyPlans/{bodyPlanId}",
                        bodyPlanId,
                        $"Body plan '{bodyPlanId}' is not defined."),
                ]);
        }

        var groupIds = bodyPlan.RequiredGroupIds
            .Concat(bodyPlan.OptionalGroupIds)
            .Concat(bodyPlan.SharedGroupIds)
            .Distinct()
            .Order()
            .ToArray();

        var groupResults = groupIds
            .Select(groupId => GroupCompletenessEvaluator.Evaluate(definition, genomeState, groupId))
            .ToArray();

        var requiredFailures = groupResults
            .Where(result => bodyPlan.RequiredGroupIds.Contains(result.GroupId) && !result.IsComplete)
            .ToArray();

        if (requiredFailures.Length > 0)
        {
            return new BodyPlanAvailabilityResult(bodyPlanId, BodyPlanAvailabilityStatus.Incomplete, groupResults);
        }

        return new BodyPlanAvailabilityResult(
            bodyPlanId,
            expressionState.IsActive(bodyPlanId)
                ? BodyPlanAvailabilityStatus.Active
                : BodyPlanAvailabilityStatus.Dormant,
            groupResults);
    }
}
