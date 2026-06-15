using Genomancy.Core.ResourceTesting;

namespace Genomancy.Storage.JsonFile;

public sealed class ResourceTestResultManifestJsonFileUpdater
{
    private readonly JsonFileStore<ResourceTestResultManifest> _store;

    public ResourceTestResultManifestJsonFileUpdater(JsonFileStore<ResourceTestResultManifest>? store = null)
    {
        _store = store ?? ResourceTestResultManifestJsonFileStore.Create();
    }

    public ResourceTestResultManifest AppendEntries(
        string path,
        IEnumerable<ResourceTestResultManifestEntry> entries)
    {
        var existing = LoadExisting(path);
        var updated = ResourceTestResultManifestMerger.Merge(existing, entries);

        _store.Save(path, updated);
        return updated;
    }

    public ResourceTestResultManifest AppendManifest(
        string path,
        ResourceTestResultManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        return AppendEntries(path, manifest.Entries);
    }

    public ResourceTestResultManifest AppendBatchResult(
        string path,
        ResourceTestBatchRunResult batchResult)
    {
        ArgumentNullException.ThrowIfNull(batchResult);

        return AppendEntries(path, batchResult.Manifest.Entries);
    }

    public ResourceTestResultManifest UpsertEntries(
        string path,
        IEnumerable<ResourceTestResultManifestEntry> entries)
    {
        var existing = LoadExisting(path);
        var updated = ResourceTestResultManifestMerger.Upsert(existing, entries);

        _store.Save(path, updated);
        return updated;
    }

    public ResourceTestResultManifest UpsertManifest(
        string path,
        ResourceTestResultManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        return UpsertEntries(path, manifest.Entries);
    }

    public ResourceTestResultManifest UpsertBatchResult(
        string path,
        ResourceTestBatchRunResult batchResult)
    {
        ArgumentNullException.ThrowIfNull(batchResult);

        return UpsertEntries(path, batchResult.Manifest.Entries);
    }

    private ResourceTestResultManifest LoadExisting(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("JSON file path must not be empty.", nameof(path));
        }

        return File.Exists(path)
            ? _store.Load(path)
            : new ResourceTestResultManifest([]);
    }
}
