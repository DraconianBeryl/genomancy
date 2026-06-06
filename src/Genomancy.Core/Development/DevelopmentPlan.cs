namespace Genomancy.Core.Development;

public sealed record DevelopmentPlan
{
    public DevelopmentPlan(IEnumerable<DevelopmentStageDefinition> stages)
    {
        ArgumentNullException.ThrowIfNull(stages);

        Stages = stages
            .OrderBy(stage => stage.Order)
            .ThenBy(stage => stage.Id)
            .ToArray();

        if (Stages.Count == 0)
        {
            throw new ArgumentException("Development plan must contain at least one stage.", nameof(stages));
        }
    }

    public IReadOnlyList<DevelopmentStageDefinition> Stages { get; }
}
