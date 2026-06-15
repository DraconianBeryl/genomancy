namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestResultManifestMerger
{
    public static ResourceTestResultManifest Merge(
        ResourceTestResultManifest? existing,
        params ResourceTestResultManifest[] manifests)
    {
        ArgumentNullException.ThrowIfNull(manifests);

        return Merge(
            existing,
            manifests
                .Where(manifest => manifest is not null)
                .SelectMany(manifest => manifest.Entries));
    }

    public static ResourceTestResultManifest Merge(
        ResourceTestResultManifest? existing,
        params ResourceTestBatchRunResult[] batchResults)
    {
        ArgumentNullException.ThrowIfNull(batchResults);

        return Merge(
            existing,
            batchResults
                .Where(result => result is not null)
                .SelectMany(result => result.Manifest.Entries));
    }

    public static ResourceTestResultManifest Merge(
        ResourceTestResultManifest? existing,
        IEnumerable<ResourceTestResultManifestEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        var existingEntries = existing?.Entries ?? [];
        var incomingEntries = entries.ToArray();
        var duplicateRunIds = incomingEntries
            .GroupBy(entry => entry.RunId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key.Value)
            .Order(StringComparer.Ordinal)
            .ToArray();

        if (duplicateRunIds.Length > 0)
        {
            throw new ArgumentException(
                $"Resource test result manifest merge contains duplicate incoming run ids: {string.Join(", ", duplicateRunIds)}.",
                nameof(entries));
        }

        return new ResourceTestResultManifest(existingEntries.Concat(incomingEntries));
    }

    public static ResourceTestResultManifest Upsert(
        ResourceTestResultManifest? existing,
        params ResourceTestResultManifest[] manifests)
    {
        ArgumentNullException.ThrowIfNull(manifests);

        return Upsert(
            existing,
            manifests
                .Where(manifest => manifest is not null)
                .SelectMany(manifest => manifest.Entries));
    }

    public static ResourceTestResultManifest Upsert(
        ResourceTestResultManifest? existing,
        params ResourceTestBatchRunResult[] batchResults)
    {
        ArgumentNullException.ThrowIfNull(batchResults);

        return Upsert(
            existing,
            batchResults
                .Where(result => result is not null)
                .SelectMany(result => result.Manifest.Entries));
    }

    public static ResourceTestResultManifest Upsert(
        ResourceTestResultManifest? existing,
        IEnumerable<ResourceTestResultManifestEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        var byRunId = new Dictionary<ResourceTestId, ResourceTestResultManifestEntry>();

        foreach (var entry in existing?.Entries ?? [])
        {
            byRunId[entry.RunId] = entry;
        }

        foreach (var entry in entries)
        {
            byRunId[entry.RunId] = entry;
        }

        return new ResourceTestResultManifest(byRunId.Values);
    }
}
