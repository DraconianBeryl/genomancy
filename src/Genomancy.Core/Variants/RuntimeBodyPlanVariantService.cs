using Genomancy.Core.Definitions;
using Genomancy.Core.Expression;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Variants;

public static class RuntimeBodyPlanVariantService
{
    public static BodyPlanAvailabilityResult EvaluateAvailability(
        FrozenSystemDefinition definition,
        GenomeState genomeState,
        BodyPlanExpressionState expressionState,
        RuntimeBodyPlanVariant variant)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(genomeState);
        ArgumentNullException.ThrowIfNull(expressionState);
        ArgumentNullException.ThrowIfNull(variant);

        var basePlan = definition.BodyPlans.FirstOrDefault(plan => plan.Id == variant.BaseBodyPlanId);

        if (basePlan is null)
        {
            return new BodyPlanAvailabilityResult(
                variant.BaseBodyPlanId,
                BodyPlanAvailabilityStatus.Unavailable,
                diagnostics:
                [
                    new ExpressionDiagnostic(
                        "BODY_PLAN_VARIANT_UNKNOWN_BASE",
                        $"bodyPlanVariants/{variant.Id}/baseBodyPlan/{variant.BaseBodyPlanId}",
                        variant.BaseBodyPlanId,
                        $"Variant base body plan '{variant.BaseBodyPlanId}' is not defined."),
                ]);
        }

        var requiredGroupIds = basePlan.RequiredGroupIds.Concat(variant.RequiredGroupIds).Distinct().Order().ToArray();
        var allGroupIds = requiredGroupIds
            .Concat(basePlan.OptionalGroupIds)
            .Concat(variant.OptionalGroupIds)
            .Concat(basePlan.SharedGroupIds)
            .Concat(variant.SharedGroupIds)
            .Distinct()
            .Order()
            .ToArray();
        var groupResults = allGroupIds
            .Select(groupId => GroupCompletenessEvaluator.Evaluate(definition, genomeState, groupId))
            .ToArray();

        if (groupResults.Any(result => requiredGroupIds.Contains(result.GroupId) && !result.IsComplete))
        {
            return new BodyPlanAvailabilityResult(variant.BaseBodyPlanId, BodyPlanAvailabilityStatus.Incomplete, groupResults);
        }

        return new BodyPlanAvailabilityResult(
            variant.BaseBodyPlanId,
            expressionState.IsActive(variant.BaseBodyPlanId)
                ? BodyPlanAvailabilityStatus.Active
                : BodyPlanAvailabilityStatus.Dormant,
            groupResults);
    }
}
