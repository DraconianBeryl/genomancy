namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestRunResult
{
    public ResourceTestRunResult(IEnumerable<ResourceTestCaseResult> cases)
    {
        ArgumentNullException.ThrowIfNull(cases);

        Cases = Array.AsReadOnly(cases.OrderBy(result => result.TestId).ToArray());
    }

    public IReadOnlyList<ResourceTestCaseResult> Cases { get; }

    public ResourceTestStatus Status => Cases.Any(result => result.Status == ResourceTestStatus.Failed)
        ? ResourceTestStatus.Failed
        : ResourceTestStatus.Passed;
}
