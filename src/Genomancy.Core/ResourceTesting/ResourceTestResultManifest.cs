namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestResultManifest
{
    public ResourceTestResultManifest(IEnumerable<ResourceTestResultManifestEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        var orderedEntries = entries
            .OrderBy(entry => entry.RunId)
            .ThenBy(entry => entry.ResultPath, StringComparer.Ordinal)
            .ToArray();

        var duplicateRunIds = orderedEntries
            .GroupBy(entry => entry.RunId)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key.Value)
            .ToArray();

        if (duplicateRunIds.Length > 0)
        {
            throw new ArgumentException(
                $"Resource test result manifest contains duplicate run ids: {string.Join(", ", duplicateRunIds)}.",
                nameof(entries));
        }

        Entries = Array.AsReadOnly(orderedEntries);
    }

    public IReadOnlyList<ResourceTestResultManifestEntry> Entries { get; }
}
