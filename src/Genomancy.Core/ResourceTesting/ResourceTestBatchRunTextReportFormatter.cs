using System.Text;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestBatchRunTextReportFormatter
{
    public static string WriteToText(ResourceTestBatchRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var builder = new StringBuilder();
        var summary = ResourceTestBatchRunSummary.FromResult(result);

        builder.AppendLine($"Resource test batch: {summary.Status}");
        builder.AppendLine($"Runs: {summary.TotalRuns} total, {summary.PassedRuns} passed, {summary.FailedRuns} failed");
        builder.AppendLine($"Cases: {summary.TotalCases} total, {summary.PassedCases} passed, {summary.FailedCases} failed");
        builder.AppendLine(
            $"Diagnostics: {summary.TotalDiagnostics} total, {summary.ErrorDiagnostics} error, {summary.WarningDiagnostics} warning, {summary.InfoDiagnostics} info");
        builder.AppendLine($"Reproducibility packets: {summary.ReproducibilityPackets}");
        builder.AppendLine("Runs:");

        foreach (var run in result.Runs)
        {
            var runSummary = run.ManifestEntry.Summary;

            builder.AppendLine(
                $"- {run.RunId}: {runSummary.Status} path={run.ResultPath} cases={runSummary.TotalCases} passed={runSummary.PassedCases} failed={runSummary.FailedCases}");
            builder.AppendLine(
                $"  Diagnostics: {runSummary.TotalDiagnostics} total, {runSummary.ErrorDiagnostics} error, {runSummary.WarningDiagnostics} warning, {runSummary.InfoDiagnostics} info");
            builder.AppendLine($"  Reproducibility packets: {runSummary.ReproducibilityPackets}");
            builder.AppendLine($"  Tags: {FormatTags(run.ManifestEntry.Tags)}");

            if (string.IsNullOrWhiteSpace(run.ManifestEntry.Label))
            {
                builder.AppendLine("  Label: none");
            }
            else
            {
                builder.AppendLine($"  Label: {run.ManifestEntry.Label}");
            }
        }

        builder.AppendLine("Manifest entries:");

        foreach (var entry in result.Manifest.Entries)
        {
            builder.AppendLine(
                $"- {entry.RunId}: {entry.ResultPath} completed={FormatCompletedAt(entry.CompletedAtUtc)} status={entry.Summary.Status}");
        }

        return builder.ToString();
    }

    private static string FormatCompletedAt(DateTimeOffset? completedAtUtc)
    {
        return completedAtUtc is null
            ? "none"
            : completedAtUtc.Value.ToUniversalTime().ToString("O");
    }

    private static string FormatTags(IReadOnlyCollection<string> tags)
    {
        return tags.Count == 0
            ? "none"
            : string.Join(", ", tags);
    }
}
