namespace Genomancy.Core.Simulation;

public sealed record PopulationTemplateSimulationResult(
    int SampleCount,
    int ObservationCount,
    int MatchingObservationCount,
    double ObservedProportion,
    StatisticalTolerance Tolerance,
    ReproducibilityPacket? FailurePacket)
{
    public bool IsWithinTolerance => FailurePacket is null;
}
