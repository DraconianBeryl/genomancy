using Genomancy.Core.Definitions;

namespace Genomancy.Core.Inheritance;

public static class TraceUpdateService
{
    public static HeritableObjectState ActivateTrace(HeritableObjectState state, ResourceId traceId)
    {
        ArgumentNullException.ThrowIfNull(state);

        var trace = state.Traces.FirstOrDefault(value => value.Id == traceId)
            ?? throw new ArgumentException($"Trace '{traceId}' is missing.", nameof(traceId));

        return state.WithTrace(trace.Activate());
    }

    public static HeritableObjectState ReplaceTraceStrength(HeritableObjectState state, ResourceId traceId, double strength)
    {
        ArgumentNullException.ThrowIfNull(state);

        var trace = state.Traces.FirstOrDefault(value => value.Id == traceId)
            ?? throw new ArgumentException($"Trace '{traceId}' is missing.", nameof(traceId));

        return state.WithTrace(trace.ReplaceStrength(strength));
    }

    public static HeritableObjectState DegradeTraces(HeritableObjectState state, int steps = 1)
    {
        ArgumentNullException.ThrowIfNull(state);

        return new HeritableObjectState(
            state.NonPloidalObjects,
            state.Traces.Select(trace => trace.Degrade(steps)));
    }
}
