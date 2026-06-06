using Genomancy.Core.Genome;

namespace Genomancy.Core.Development;

public static class DevelopmentService
{
    public static DevelopmentTimeline CreateTimeline(
        DevelopmentPlan plan,
        GestationContext? gestationContext = null)
    {
        ArgumentNullException.ThrowIfNull(plan);

        var diagnostics = new List<string>();

        if (plan.Stages.Any(stage => stage.RequiresGestation) && gestationContext is null)
        {
            diagnostics.Add("DEVELOPMENT_GESTATION_REQUIRED");
        }

        return new DevelopmentTimeline(plan.Stages, gestationContext, diagnostics);
    }

    public static GenomeState ApplyMaternalEffects(GenomeState state, IEnumerable<MaternalEffect> effects)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(effects);

        var current = state;

        foreach (var effect in effects)
        {
            var group = current.Groups.FirstOrDefault(value => value.GroupId == effect.GroupId)
                ?? throw new ArgumentException($"Group '{effect.GroupId}' is missing.", nameof(effects));
            var set = group.GeneAlleles.FirstOrDefault(value => value.GeneId == effect.GeneId)
                ?? throw new ArgumentException($"Gene '{effect.GeneId}' is missing in group '{effect.GroupId}'.", nameof(effects));
            var entries = set.Entries
                .Select(entry => entry.Rank == effect.AlleleRank
                    ? new RankedAlleleEntry(entry.AlleleId, entry.Rank, (entry.NumericValue ?? 0) + effect.NumericDelta)
                    : entry)
                .ToArray();

            current = current.WithGeneAlleles(effect.GroupId, new RankedAlleleSet(effect.GeneId, entries));
        }

        return current;
    }
}
