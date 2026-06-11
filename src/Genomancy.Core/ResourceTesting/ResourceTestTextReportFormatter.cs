using System.Text;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestTextReportFormatter
{
    public static string WriteToText(ResourceTestRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var builder = new StringBuilder();
        var summary = ResourceTestRunSummary.FromResult(result);

        builder.AppendLine($"Resource test run: {summary.Status}");
        builder.AppendLine($"Cases: {summary.TotalCases} total, {summary.PassedCases} passed, {summary.FailedCases} failed");
        builder.AppendLine(
            $"Diagnostics: {summary.TotalDiagnostics} total, {summary.ErrorDiagnostics} error, {summary.WarningDiagnostics} warning, {summary.InfoDiagnostics} info");
        builder.AppendLine($"Reproducibility packets: {summary.ReproducibilityPackets}");
        builder.AppendLine("Cases:");

        foreach (var testCase in result.Cases)
        {
            builder.AppendLine($"- {testCase.TestId}: {testCase.Status} [tags: {FormatTags(testCase.Tags)}]");

            if (testCase.Diagnostics.Count == 0)
            {
                builder.AppendLine("  Diagnostics: none");
            }
            else
            {
                builder.AppendLine("  Diagnostics:");
                foreach (var diagnostic in testCase.Diagnostics)
                {
                    builder.AppendLine(
                        $"  - {diagnostic.Severity} {diagnostic.Code} {diagnostic.Path}: {diagnostic.Message}");
                }
            }

            if (testCase.ReproducibilityPackets.Count == 0)
            {
                builder.AppendLine("  Reproducibility packets: none");
            }
            else
            {
                builder.AppendLine("  Reproducibility packets:");
                foreach (var packet in testCase.ReproducibilityPackets)
                {
                    builder.AppendLine(
                        $"  - {packet.OperationPath} seed={packet.Seed} resourceSet={packet.ResourceSetVersion} assertion={packet.FailureAssertion}");
                }
            }
        }

        return builder.ToString();
    }

    private static string FormatTags(IReadOnlyCollection<string> tags)
    {
        return tags.Count == 0
            ? "none"
            : string.Join(", ", tags);
    }
}
