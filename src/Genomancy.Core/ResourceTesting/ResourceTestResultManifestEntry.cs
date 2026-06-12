namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestResultManifestEntry
{
    public ResourceTestResultManifestEntry(
        ResourceTestId runId,
        string resultPath,
        ResourceTestRunSummary summary,
        DateTimeOffset? completedAtUtc = null,
        string? label = null,
        IEnumerable<string>? tags = null)
    {
        if (string.IsNullOrWhiteSpace(resultPath))
        {
            throw new ArgumentException("Resource test result manifest path must not be empty.", nameof(resultPath));
        }

        RunId = runId;
        ResultPath = resultPath.Trim();
        Summary = summary ?? throw new ArgumentNullException(nameof(summary));
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

    public ResourceTestRunSummary Summary { get; }

    public DateTimeOffset? CompletedAtUtc { get; }

    public string? Label { get; }

    public IReadOnlyList<string> Tags { get; }

    public static ResourceTestResultManifestEntry FromResult(
        ResourceTestId runId,
        string resultPath,
        ResourceTestRunResult result,
        DateTimeOffset? completedAtUtc = null,
        string? label = null,
        IEnumerable<string>? tags = null)
    {
        ArgumentNullException.ThrowIfNull(result);

        return new ResourceTestResultManifestEntry(
            runId,
            resultPath,
            ResourceTestRunSummary.FromResult(result),
            completedAtUtc,
            label,
            tags);
    }
}
