namespace Genomancy.Core.Development;

public sealed record DevelopmentTimeline
{
    public DevelopmentTimeline(
        IEnumerable<DevelopmentStageDefinition> stages,
        GestationContext? gestationContext,
        IEnumerable<string>? diagnostics = null)
    {
        Stages = stages
            .OrderBy(stage => stage.Order)
            .ThenBy(stage => stage.Id)
            .ToArray();
        GestationContext = gestationContext;
        Diagnostics = (diagnostics ?? []).Order(StringComparer.Ordinal).ToArray();
    }

    public IReadOnlyList<DevelopmentStageDefinition> Stages { get; }

    public GestationContext? GestationContext { get; }

    public IReadOnlyList<string> Diagnostics { get; }

    public bool IsValid => Diagnostics.Count == 0;
}
