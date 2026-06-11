using System.Text;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestTextReportFormatter
{
    public static string WriteToText(ResourceTestRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var builder = new StringBuilder();
        var totalCases = result.Cases.Count;
        var passedCases = result.Cases.Count(testCase => testCase.Status == ResourceTestStatus.Passed);
        var failedCases = result.Cases.Count(testCase => testCase.Status == ResourceTestStatus.Failed);
        var diagnostics = result.Cases.SelectMany(testCase => testCase.Diagnostics).ToArray();
        var packetCount = result.Cases.Sum(testCase => testCase.ReproducibilityPackets.Count);

        builder.AppendLine($"Resource test run: {result.Status}");
        builder.AppendLine($"Cases: {totalCases} total, {passedCases} passed, {failedCases} failed");
        builder.AppendLine(
            $"Diagnostics: {diagnostics.Length} total, {Count(diagnostics, ResourceTestSeverity.Error)} error, {Count(diagnostics, ResourceTestSeverity.Warning)} warning, {Count(diagnostics, ResourceTestSeverity.Info)} info");
        builder.AppendLine($"Reproducibility packets: {packetCount}");
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

    private static int Count(IEnumerable<ResourceTestDiagnostic> diagnostics, ResourceTestSeverity severity)
    {
        return diagnostics.Count(diagnostic => diagnostic.Severity == severity);
    }

    private static string FormatTags(IReadOnlyCollection<string> tags)
    {
        return tags.Count == 0
            ? "none"
            : string.Join(", ", tags);
    }
}
