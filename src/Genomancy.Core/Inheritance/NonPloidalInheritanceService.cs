using Genomancy.Core.Genome;
using Genomancy.Core.Randomness;
using Genomancy.Core.Reproduction;

namespace Genomancy.Core.Inheritance;

public static class NonPloidalInheritanceService
{
    public static NonPloidalInheritanceResult Inherit(
        IEnumerable<ReproductionParentRole> parentRoles,
        ulong seed,
        bool includeInactiveObjects = false,
        double inheritedTraceDegradationSteps = 0)
    {
        ArgumentNullException.ThrowIfNull(parentRoles);

        if (double.IsNaN(inheritedTraceDegradationSteps) || double.IsInfinity(inheritedTraceDegradationSteps) || inheritedTraceDegradationSteps < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(inheritedTraceDegradationSteps),
                inheritedTraceDegradationSteps,
                "Inherited trace degradation steps must be finite and zero or greater.");
        }

        var roles = parentRoles.ToArray();
        var objects = SelectObjects(roles, seed, includeInactiveObjects);
        var traces = roles
            .SelectMany(role => role.GenomeVersion.HeritableObjects.Traces)
            .GroupBy(trace => trace.Id)
            .Select(group => group.OrderByDescending(trace => trace.Strength).ThenBy(trace => trace.SourceId).First())
            .Select(trace => trace.Degrade((int)inheritedTraceDegradationSteps))
            .Where(trace => trace.Strength > 0)
            .OrderBy(trace => trace.Id)
            .ToArray();

        return new NonPloidalInheritanceResult(new HeritableObjectState(objects, traces));
    }

    private static IReadOnlyList<NonPloidalObjectState> SelectObjects(
        IReadOnlyList<ReproductionParentRole> roles,
        ulong seed,
        bool includeInactiveObjects)
    {
        return roles
            .SelectMany(role => role.GenomeVersion.HeritableObjects.NonPloidalObjects.Select(value => new WeightedObject(role.Name, value)))
            .Where(value => includeInactiveObjects || value.State.IsActive)
            .GroupBy(value => value.State.Id)
            .Select(group => SelectObject(group.Key.Value, group.ToArray(), seed))
            .Where(value => value is not null)
            .Select(value => value ?? throw new InvalidOperationException("Filtered null value was unexpectedly null."))
            .OrderBy(value => value.Id)
            .ToArray();
    }

    private static NonPloidalObjectState? SelectObject(
        string objectId,
        IReadOnlyList<WeightedObject> candidates,
        ulong seed)
    {
        var weightedCandidates = candidates
            .Where(candidate => candidate.State.TransmissionWeight > 0)
            .OrderBy(candidate => candidate.RoleName, StringComparer.Ordinal)
            .ThenBy(candidate => candidate.State.Id)
            .ToArray();

        if (weightedCandidates.Length == 0)
        {
            return null;
        }

        var totalWeight = weightedCandidates.Sum(candidate => candidate.State.TransmissionWeight);
        var stream = DeterministicRandomStream.FromSeedAndName(seed, $"nonploidal:{objectId}");
        var threshold = stream.NextUnitDouble() * totalWeight;
        var running = 0.0;

        foreach (var candidate in weightedCandidates)
        {
            running += candidate.State.TransmissionWeight;

            if (threshold < running)
            {
                return candidate.State;
            }
        }

        return weightedCandidates[^1].State;
    }

    private sealed record WeightedObject(string RoleName, NonPloidalObjectState State);
}
