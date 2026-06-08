namespace Genomancy.Core.Simulation;

public sealed record StatisticalTolerance
{
    public StatisticalTolerance(double expectedProportion, double absoluteTolerance)
    {
        ValidateProportion(expectedProportion, nameof(expectedProportion));

        if (double.IsNaN(absoluteTolerance)
            || double.IsInfinity(absoluteTolerance)
            || absoluteTolerance < 0
            || absoluteTolerance > 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(absoluteTolerance),
                absoluteTolerance,
                "Absolute tolerance must be between zero and one.");
        }

        ExpectedProportion = expectedProportion;
        AbsoluteTolerance = absoluteTolerance;
    }

    public double ExpectedProportion { get; }

    public double AbsoluteTolerance { get; }

    public double Minimum => Math.Max(0, ExpectedProportion - AbsoluteTolerance);

    public double Maximum => Math.Min(1, ExpectedProportion + AbsoluteTolerance);

    public bool Contains(double actualProportion)
    {
        ValidateProportion(actualProportion, nameof(actualProportion));
        return actualProportion >= Minimum && actualProportion <= Maximum;
    }

    private static void ValidateProportion(double value, string parameterName)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0 || value > 1)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                value,
                "Proportion must be between zero and one.");
        }
    }
}
