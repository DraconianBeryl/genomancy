namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestBatchRunRequest
{
    public ResourceTestBatchRunRequest(
        ResourceTestId runId,
        string resultPath,
        IEnumerable<ResourceTestDefinition> definitions,
        ResourceTestRunOptions? options = null,
        DateTimeOffset? completedAtUtc = null,
        string? label = null,
        IEnumerable<string>? tags = null)
    {
        if (string.IsNullOrWhiteSpace(resultPath))
        {
            throw new ArgumentException("Resource test batch result path must not be empty.", nameof(resultPath));
        }

        ArgumentNullException.ThrowIfNull(definitions);

        RunId = runId;
        ResultPath = resultPath.Trim();
        Definitions = Array.AsReadOnly(definitions.OrderBy(definition => definition.Id).ToArray());
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

    public IReadOnlyList<ResourceTestDefinition> Definitions { get; }

    public ResourceTestRunOptions Options { get; }

    public DateTimeOffset? CompletedAtUtc { get; }

    public string? Label { get; }

    public IReadOnlyList<string> Tags { get; }
}
