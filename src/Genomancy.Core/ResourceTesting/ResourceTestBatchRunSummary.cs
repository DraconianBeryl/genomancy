namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestBatchRunSummary(
    ResourceTestStatus Status,
    int TotalRuns,
    int PassedRuns,
    int FailedRuns,
    int TotalCases,
    int PassedCases,
    int FailedCases,
    int TotalDiagnostics,
    int ErrorDiagnostics,
    int WarningDiagnostics,
    int InfoDiagnostics,
    int ReproducibilityPackets)
{
    public static ResourceTestBatchRunSummary FromResult(ResourceTestBatchRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var runSummaries = result.Runs
            .Select(run => run.ManifestEntry.Summary)
            .ToArray();

        return new ResourceTestBatchRunSummary(
            result.Status,
            result.Runs.Count,
            result.Runs.Count(run => run.Result.Status == ResourceTestStatus.Passed),
            result.Runs.Count(run => run.Result.Status == ResourceTestStatus.Failed),
            runSummaries.Sum(summary => summary.TotalCases),
            runSummaries.Sum(summary => summary.PassedCases),
            runSummaries.Sum(summary => summary.FailedCases),
            runSummaries.Sum(summary => summary.TotalDiagnostics),
            runSummaries.Sum(summary => summary.ErrorDiagnostics),
            runSummaries.Sum(summary => summary.WarningDiagnostics),
            runSummaries.Sum(summary => summary.InfoDiagnostics),
            runSummaries.Sum(summary => summary.ReproducibilityPackets));
    }
}
