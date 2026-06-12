namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestBatchRunResult
{
    public ResourceTestBatchRunResult(IEnumerable<ResourceTestBatchRunRecord> runs)
    {
        ArgumentNullException.ThrowIfNull(runs);

        var orderedRuns = runs
            .OrderBy(run => run.RunId)
            .ThenBy(run => run.ResultPath, StringComparer.Ordinal)
            .ToArray();

        Runs = Array.AsReadOnly(orderedRuns);
        Manifest = new ResourceTestResultManifest(orderedRuns.Select(run => run.ManifestEntry));
    }

    public IReadOnlyList<ResourceTestBatchRunRecord> Runs { get; }

    public ResourceTestResultManifest Manifest { get; }

    public ResourceTestStatus Status => Runs.Any(run => run.Result.Status == ResourceTestStatus.Failed)
        ? ResourceTestStatus.Failed
        : ResourceTestStatus.Passed;
}
