using Genomancy.Core.Definitions;

namespace Genomancy.Core.Inheritance;

public sealed record HeritableObjectState
{
    public HeritableObjectState(
        IEnumerable<NonPloidalObjectState>? nonPloidalObjects = null,
        IEnumerable<TraceState>? traces = null)
    {
        NonPloidalObjects = (nonPloidalObjects ?? [])
            .OrderBy(value => value.Id)
            .ToReadOnlyList();
        Traces = (traces ?? [])
            .OrderBy(value => value.Id)
            .ToReadOnlyList();
    }

    public IReadOnlyList<NonPloidalObjectState> NonPloidalObjects { get; }

    public IReadOnlyList<TraceState> Traces { get; }

    public HeritableObjectState WithObject(NonPloidalObjectState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        return new HeritableObjectState(
            NonPloidalObjects.Where(existing => existing.Id != state.Id).Append(state),
            Traces);
    }

    public HeritableObjectState WithTrace(TraceState trace)
    {
        ArgumentNullException.ThrowIfNull(trace);

        return new HeritableObjectState(
            NonPloidalObjects,
            Traces.Where(existing => existing.Id != trace.Id).Append(trace));
    }

    public bool Equals(HeritableObjectState? other)
    {
        return other is not null
            && NonPloidalObjects.SequenceEqual(other.NonPloidalObjects)
            && Traces.SequenceEqual(other.Traces);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var value in NonPloidalObjects)
        {
            hash.Add(value);
        }

        foreach (var trace in Traces)
        {
            hash.Add(trace);
        }

        return hash.ToHashCode();
    }
}
