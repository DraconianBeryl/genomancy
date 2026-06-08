namespace Genomancy.Core.Simulation;

public sealed record SimulationResourceLimits
{
    public SimulationResourceLimits(int maximumSamples = 100_000)
    {
        if (maximumSamples <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumSamples),
                maximumSamples,
                "Maximum samples must be greater than zero.");
        }

        MaximumSamples = maximumSamples;
    }

    public int MaximumSamples { get; }

    public void ValidateSampleCount(int sampleCount)
    {
        if (sampleCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sampleCount),
                sampleCount,
                "Simulation sample count must be greater than zero.");
        }

        if (sampleCount > MaximumSamples)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sampleCount),
                sampleCount,
                $"Simulation sample count exceeds the configured maximum of {MaximumSamples}.");
        }
    }
}
