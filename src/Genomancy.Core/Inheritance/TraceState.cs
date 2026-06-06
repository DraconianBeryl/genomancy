using Genomancy.Core.Definitions;

namespace Genomancy.Core.Inheritance;

public sealed record TraceState
{
    public TraceState(
        ResourceId id,
        ResourceId sourceId,
        double strength,
        bool isActive = false,
        int age = 0,
        double degradationPerStep = 0)
    {
        if (double.IsNaN(strength) || double.IsInfinity(strength) || strength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(strength), strength, "Trace strength must be finite and zero or greater.");
        }

        if (age < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(age), age, "Trace age must be zero or greater.");
        }

        if (double.IsNaN(degradationPerStep) || double.IsInfinity(degradationPerStep) || degradationPerStep < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(degradationPerStep), degradationPerStep, "Trace degradation must be finite and zero or greater.");
        }

        Id = id;
        SourceId = sourceId;
        Strength = strength;
        IsActive = isActive;
        Age = age;
        DegradationPerStep = degradationPerStep;
    }

    public ResourceId Id { get; }

    public ResourceId SourceId { get; }

    public double Strength { get; }

    public bool IsActive { get; }

    public int Age { get; }

    public double DegradationPerStep { get; }

    public TraceState Activate()
    {
        return new TraceState(Id, SourceId, Strength, isActive: true, Age, DegradationPerStep);
    }

    public TraceState Deactivate()
    {
        return new TraceState(Id, SourceId, Strength, isActive: false, Age, DegradationPerStep);
    }

    public TraceState ReplaceStrength(double strength)
    {
        return new TraceState(Id, SourceId, strength, IsActive, Age, DegradationPerStep);
    }

    public TraceState Degrade(int steps = 1)
    {
        if (steps < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(steps), steps, "Degradation steps must be zero or greater.");
        }

        return new TraceState(
            Id,
            SourceId,
            Math.Max(0, Strength - (DegradationPerStep * steps)),
            IsActive,
            Age + steps,
            DegradationPerStep);
    }
}
