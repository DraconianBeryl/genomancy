namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestBatchRunRecord
{
    public ResourceTestBatchRunRecord(
        ResourceTestId runId,
        string resultPath,
        ResourceTestRunResult result,
        ResourceTestResultManifestEntry manifestEntry)
    {
        if (string.IsNullOrWhiteSpace(resultPath))
        {
            throw new ArgumentException("Resource test batch result path must not be empty.", nameof(resultPath));
        }

        RunId = runId;
        ResultPath = resultPath.Trim();
        Result = result ?? throw new ArgumentNullException(nameof(result));
        ManifestEntry = manifestEntry ?? throw new ArgumentNullException(nameof(manifestEntry));
    }

    public ResourceTestId RunId { get; }

    public string ResultPath { get; }

    public ResourceTestRunResult Result { get; }

    public ResourceTestResultManifestEntry ManifestEntry { get; }
}
