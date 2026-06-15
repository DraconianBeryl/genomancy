using Genomancy.Core.ResourceTesting;

namespace Genomancy.Storage.JsonFile;

public sealed record ResourceTestBatchRunJsonFileExecutionResult
{
    public ResourceTestBatchRunJsonFileExecutionResult(
        string planPath,
        string batchResultPath,
        ResourceTestBatchRunResult batchResult,
        IEnumerable<string> writtenRunResultPaths,
        string? manifestPath = null,
        ResourceTestResultManifest? updatedManifest = null)
    {
        if (string.IsNullOrWhiteSpace(planPath))
        {
            throw new ArgumentException("JSON batch plan path must not be empty.", nameof(planPath));
        }

        if (string.IsNullOrWhiteSpace(batchResultPath))
        {
            throw new ArgumentException("JSON batch result path must not be empty.", nameof(batchResultPath));
        }

        ArgumentNullException.ThrowIfNull(writtenRunResultPaths);

        PlanPath = planPath.Trim();
        BatchResultPath = batchResultPath.Trim();
        BatchResult = batchResult ?? throw new ArgumentNullException(nameof(batchResult));
        WrittenRunResultPaths = Array.AsReadOnly(writtenRunResultPaths
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(path => path.Trim())
            .Order(StringComparer.Ordinal)
            .ToArray());
        ManifestPath = string.IsNullOrWhiteSpace(manifestPath) ? null : manifestPath.Trim();
        UpdatedManifest = updatedManifest;
    }

    public string PlanPath { get; }

    public string BatchResultPath { get; }

    public ResourceTestBatchRunResult BatchResult { get; }

    public IReadOnlyList<string> WrittenRunResultPaths { get; }

    public string? ManifestPath { get; }

    public ResourceTestResultManifest? UpdatedManifest { get; }
}
