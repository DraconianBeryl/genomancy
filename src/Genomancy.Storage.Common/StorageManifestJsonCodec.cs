using System.Text.Json;
using System.Text.Json.Serialization;

namespace Genomancy.Storage.Common;

public static class StorageManifestJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static void Write(Stream stream, StorageManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(manifest);

        JsonSerializer.Serialize(stream, ToEnvelope(manifest), JsonOptions);
    }

    public static string WriteToText(StorageManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        return JsonSerializer.Serialize(ToEnvelope(manifest), JsonOptions);
    }

    public static byte[] WriteToBuffer(StorageManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(manifest), JsonOptions);
    }

    public static StorageManifest Read(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var envelope = JsonSerializer.Deserialize<ManifestEnvelope>(stream, JsonOptions)
                ?? throw new StorageManifestException("Storage manifest JSON envelope was empty.");

            return FromEnvelope(envelope);
        }
        catch (JsonException exception)
        {
            throw new StorageManifestException("Storage manifest JSON is malformed.", exception);
        }
    }

    public static StorageManifest ReadFromBuffer(ReadOnlySpan<byte> buffer)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream);
    }

    private static ManifestEnvelope ToEnvelope(StorageManifest manifest)
    {
        return new ManifestEnvelope(
            EnvelopeVersion,
            manifest.SystemDefinitionVersion,
            manifest.ChangeSummary,
            manifest.Entries
                .OrderBy(entry => entry.RelativePath, StringComparer.Ordinal)
                .ThenBy(entry => entry.Kind)
                .ThenBy(entry => entry.Format)
                .Select(entry => new EntryEnvelope(
                    entry.Kind.ToString(),
                    entry.Format.ToString(),
                    entry.RelativePath,
                    entry.FullPath,
                    entry.ResourceId,
                    entry.ResourceVersionId,
                    entry.SystemDefinitionVersion,
                    entry.ByteCount,
                    entry.Sha256Hex))
                .ToArray());
    }

    private static StorageManifest FromEnvelope(ManifestEnvelope envelope)
    {
        if (envelope.EnvelopeVersion != EnvelopeVersion)
        {
            throw new StorageManifestException($"Unsupported storage manifest envelope version '{envelope.EnvelopeVersion}'.");
        }

        return new StorageManifest(
            Required(envelope.SystemDefinitionVersion, "systemDefinitionVersion"),
            (envelope.Entries ?? []).Select(entry => new StoredResourceEntry(
                Enum.Parse<StoredResourceKind>(Required(entry.Kind, "entry.kind")),
                Enum.Parse<StoredResourceFormat>(Required(entry.Format, "entry.format")),
                Required(entry.RelativePath, "entry.relativePath"),
                Required(entry.FullPath, "entry.fullPath"),
                Required(entry.ResourceId, "entry.resourceId"),
                Required(entry.ResourceVersionId, "entry.resourceVersionId"),
                Required(entry.SystemDefinitionVersion, "entry.systemDefinitionVersion"),
                entry.ByteCount,
                Required(entry.Sha256Hex, "entry.sha256Hex"))),
            envelope.ChangeSummary ?? string.Empty);
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new StorageManifestException($"Storage manifest JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record ManifestEnvelope(
        int EnvelopeVersion,
        string? SystemDefinitionVersion,
        string? ChangeSummary,
        IReadOnlyList<EntryEnvelope>? Entries);

    private sealed record EntryEnvelope(
        string? Kind,
        string? Format,
        string? RelativePath,
        string? FullPath,
        string? ResourceId,
        string? ResourceVersionId,
        string? SystemDefinitionVersion,
        long ByteCount,
        string? Sha256Hex);
}
