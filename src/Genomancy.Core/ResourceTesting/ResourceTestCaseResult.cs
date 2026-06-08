namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestCaseResult
{
    public ResourceTestCaseResult(
        ResourceTestId testId,
        ResourceTestStatus status,
        IEnumerable<ResourceTestDiagnostic> diagnostics,
        IEnumerable<string>? tags = null)
    {
        ArgumentNullException.ThrowIfNull(diagnostics);

        TestId = testId;
        Status = status;
        Diagnostics = Array.AsReadOnly(diagnostics.Order().ToArray());
        Tags = Array.AsReadOnly((tags ?? []).Order(StringComparer.Ordinal).ToArray());
    }

    public ResourceTestId TestId { get; }

    public ResourceTestStatus Status { get; }

    public IReadOnlyList<ResourceTestDiagnostic> Diagnostics { get; }

    public IReadOnlyList<string> Tags { get; }
}
