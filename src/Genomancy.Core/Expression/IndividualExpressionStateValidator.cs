using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Expression;

public static class IndividualExpressionStateValidator
{
    public static bool HasAtLeastOneActiveAvailableBodyPlan(
        FrozenSystemDefinition definition,
        GenomeState genomeState,
        BodyPlanExpressionState expressionState,
        DevelopmentalPhaseId developmentalPhaseId,
        ExpressionExternalContext externalContext)
    {
        ArgumentNullException.ThrowIfNull(definition);

        return definition.BodyPlans.Any(bodyPlan =>
            BodyPlanAvailabilityEvaluator.Evaluate(
                definition,
                genomeState,
                expressionState,
                bodyPlan.Id,
                developmentalPhaseId,
                externalContext).Status == BodyPlanAvailabilityStatus.Active);
    }
}
