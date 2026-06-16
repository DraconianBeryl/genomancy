namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestResultManifestRegenerator
{
    public static ResourceTestResultManifest FromBatchResult(ResourceTestBatchRunResult batchResult)
    {
        ArgumentNullException.ThrowIfNull(batchResult);

        return FromBatchResults([batchResult]);
    }

    public static ResourceTestResultManifest FromBatchResults(IEnumerable<ResourceTestBatchRunResult> batchResults)
    {
        ArgumentNullException.ThrowIfNull(batchResults);

        return FromRunRecords(batchResults.SelectMany(result => result.Runs));
    }

    public static ResourceTestResultManifest FromRunRecords(IEnumerable<ResourceTestBatchRunRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        return new ResourceTestResultManifest(records.Select(RegenerateEntry));
    }

    public static ResourceTestResultManifestEntry RegenerateEntry(ResourceTestBatchRunRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        return ResourceTestResultManifestEntry.FromResult(
            record.RunId,
            record.ResultPath,
            record.Result,
            record.ManifestEntry.CompletedAtUtc,
            record.ManifestEntry.Label,
            record.ManifestEntry.Tags);
    }
}
