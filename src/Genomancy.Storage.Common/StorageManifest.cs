namespace Genomancy.Storage.Common;

public sealed record StorageManifest
{
    public StorageManifest(
        string systemDefinitionVersion,
        IEnumerable<StoredResourceEntry> entries,
        string changeSummary = "")
    {
        if (string.IsNullOrWhiteSpace(systemDefinitionVersion))
        {
            throw new ArgumentException("System definition version must not be empty.", nameof(systemDefinitionVersion));
        }

        ArgumentNullException.ThrowIfNull(entries);

        SystemDefinitionVersion = systemDefinitionVersion.Trim();
        Entries = Array.AsReadOnly(entries
            .OrderBy(entry => entry.RelativePath, StringComparer.Ordinal)
            .ThenBy(entry => entry.Kind)
            .ThenBy(entry => entry.Format)
            .ToArray());
        ChangeSummary = changeSummary ?? string.Empty;

        foreach (var entry in Entries)
        {
            if (!string.Equals(entry.SystemDefinitionVersion, SystemDefinitionVersion, StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "All manifest entries must use the manifest system definition version.",
                    nameof(entries));
            }
        }
    }

    public string SystemDefinitionVersion { get; }

    public IReadOnlyList<StoredResourceEntry> Entries { get; }

    public string ChangeSummary { get; }

    public bool Equals(StorageManifest? other)
    {
        return other is not null
            && SystemDefinitionVersion == other.SystemDefinitionVersion
            && Entries.SequenceEqual(other.Entries)
            && ChangeSummary == other.ChangeSummary;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(SystemDefinitionVersion);

        foreach (var entry in Entries)
        {
            hash.Add(entry);
        }

        hash.Add(ChangeSummary);
        return hash.ToHashCode();
    }
}
