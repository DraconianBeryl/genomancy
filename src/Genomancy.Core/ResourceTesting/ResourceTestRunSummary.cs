namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestRunSummary(
    ResourceTestStatus Status,
    int TotalCases,
    int PassedCases,
    int FailedCases,
    int TotalDiagnostics,
    int ErrorDiagnostics,
    int WarningDiagnostics,
    int InfoDiagnostics,
    int ReproducibilityPackets)
{
    public static ResourceTestRunSummary FromResult(ResourceTestRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var diagnostics = result.Cases.SelectMany(testCase => testCase.Diagnostics).ToArray();

        return new ResourceTestRunSummary(
            result.Status,
            result.Cases.Count,
            result.Cases.Count(testCase => testCase.Status == ResourceTestStatus.Passed),
            result.Cases.Count(testCase => testCase.Status == ResourceTestStatus.Failed),
            diagnostics.Length,
            diagnostics.Count(diagnostic => diagnostic.Severity == ResourceTestSeverity.Error),
            diagnostics.Count(diagnostic => diagnostic.Severity == ResourceTestSeverity.Warning),
            diagnostics.Count(diagnostic => diagnostic.Severity == ResourceTestSeverity.Info),
            result.Cases.Sum(testCase => testCase.ReproducibilityPackets.Count));
    }
}
