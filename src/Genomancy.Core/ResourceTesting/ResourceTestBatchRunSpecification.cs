namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestBatchRunSpecification
{
    public ResourceTestBatchRunSpecification(
        ResourceTestId runId,
        string resultPath,
        IEnumerable<ResourceTestSpecification> tests,
        ResourceTestRunOptions? options = null,
        DateTimeOffset? completedAtUtc = null,
        string? label = null,
        IEnumerable<string>? tags = null)
    {
        if (string.IsNullOrWhiteSpace(resultPath))
        {
            throw new ArgumentException("Resource test batch result path must not be empty.", nameof(resultPath));
        }

        ArgumentNullException.ThrowIfNull(tests);

        RunId = runId;
        ResultPath = resultPath.Trim();
        Tests = Array.AsReadOnly(tests.OrderBy(test => test.Id).ToArray());
        Options = options ?? new ResourceTestRunOptions();
        CompletedAtUtc = completedAtUtc?.ToUniversalTime();
        Label = string.IsNullOrWhiteSpace(label) ? null : label.Trim();
        Tags = Array.AsReadOnly((tags ?? [])
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray());
    }

    public ResourceTestId RunId { get; }

    public string ResultPath { get; }

    public IReadOnlyList<ResourceTestSpecification> Tests { get; }

    public ResourceTestRunOptions Options { get; }

    public DateTimeOffset? CompletedAtUtc { get; }

    public string? Label { get; }

    public IReadOnlyList<string> Tags { get; }

    public ResourceTestBatchRunRequest ToRequest()
    {
        return new ResourceTestBatchRunRequest(
            RunId,
            ResultPath,
            Tests.Select(test => test.ToDefinition()),
            Options,
            CompletedAtUtc,
            Label,
            Tags);
    }
}
