using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Expression;

public sealed record ExpressionEvaluationContext
{
    public ExpressionEvaluationContext(
        ResourceId bodyPlanId,
        DevelopmentalPhaseId developmentalPhaseId,
        GenomeGroupState? groupState,
        ExpressionExternalContext externalContext)
    {
        ArgumentNullException.ThrowIfNull(externalContext);

        BodyPlanId = bodyPlanId;
        DevelopmentalPhaseId = developmentalPhaseId;
        GroupState = groupState;
        ExternalContext = externalContext;
    }

    public ResourceId BodyPlanId { get; }

    public DevelopmentalPhaseId DevelopmentalPhaseId { get; }

    public GenomeGroupState? GroupState { get; }

    public ExpressionExternalContext ExternalContext { get; }
}
