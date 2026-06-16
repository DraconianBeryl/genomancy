using Genomancy.Core.ResourceTesting;

namespace Genomancy.Storage.JsonFile;

public sealed class ResourceTestResultManifestJsonFileRegenerator
{
    private readonly JsonFileStore<ResourceTestBatchRunResult> _batchResultStore;
    private readonly JsonFileStore<ResourceTestResultManifest> _manifestStore;

    public ResourceTestResultManifestJsonFileRegenerator(
        JsonFileStore<ResourceTestBatchRunResult>? batchResultStore = null,
        JsonFileStore<ResourceTestResultManifest>? manifestStore = null)
    {
        _batchResultStore = batchResultStore ?? ResourceTestBatchRunResultJsonFileStore.Create();
        _manifestStore = manifestStore ?? ResourceTestResultManifestJsonFileStore.Create();
    }

    public ResourceTestResultManifest RegenerateFromBatchResult(
        string manifestPath,
        string batchResultPath)
    {
        return RegenerateFromBatchResults(manifestPath, [batchResultPath]);
    }

    public ResourceTestResultManifest RegenerateFromBatchResults(
        string manifestPath,
        IEnumerable<string> batchResultPaths)
    {
        if (string.IsNullOrWhiteSpace(manifestPath))
        {
            throw new ArgumentException("JSON result manifest path must not be empty.", nameof(manifestPath));
        }

        ArgumentNullException.ThrowIfNull(batchResultPaths);

        var paths = batchResultPaths
            .Select(path => string.IsNullOrWhiteSpace(path)
                ? throw new ArgumentException("JSON batch result path must not be empty.", nameof(batchResultPaths))
                : path.Trim())
            .ToArray();

        if (paths.Length == 0)
        {
            throw new ArgumentException("At least one JSON batch result path is required.", nameof(batchResultPaths));
        }

        var batchResults = paths.Select(_batchResultStore.Load).ToArray();
        var manifest = ResourceTestResultManifestRegenerator.FromBatchResults(batchResults);

        _manifestStore.Save(manifestPath, manifest);
        return manifest;
    }
}
