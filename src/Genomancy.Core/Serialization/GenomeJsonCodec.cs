using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Serialization;

public static class GenomeJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static void WriteVersion(Stream stream, GenomeVersion version)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(version);

        JsonSerializer.Serialize(stream, ToEnvelope(version), JsonOptions);
    }

    public static byte[] WriteVersionToBuffer(GenomeVersion version)
    {
        using var stream = new MemoryStream();
        WriteVersion(stream, version);
        return stream.ToArray();
    }

    public static string WriteVersionToText(GenomeVersion version)
    {
        return JsonSerializer.Serialize(ToEnvelope(version), JsonOptions);
    }

    public static GenomeVersion ReadVersion(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion,
        Func<SystemDefinitionVersion, SystemDefinitionVersion, GenomeCompatibilityResult>? compatibility = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var envelope = JsonSerializer.Deserialize<GenomeVersionEnvelope>(stream, JsonOptions)
                ?? throw new GenomeSerializationException("Genome JSON envelope was empty.");

            return FromEnvelope(envelope, expectedSystemDefinitionVersion, compatibility);
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Genome JSON is malformed.", exception);
        }
    }

    public static GenomeVersion ReadVersionFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion,
        Func<SystemDefinitionVersion, SystemDefinitionVersion, GenomeCompatibilityResult>? compatibility = null)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return ReadVersion(stream, expectedSystemDefinitionVersion, compatibility);
    }

    private static GenomeVersionEnvelope ToEnvelope(GenomeVersion version)
    {
        return new GenomeVersionEnvelope(
            EnvelopeVersion,
            version.Id.Value,
            version.SystemDefinitionVersion.Value,
            version.IndividualId.Value,
            version.ParentVersionId?.Value,
            version.ChangeSummary,
            version.State.Groups
                .OrderBy(group => group.GroupId)
                .Select(group => new GenomeGroupEnvelope(
                    group.GroupId.Value,
                    group.GeneAlleles
                        .OrderBy(set => set.GeneId)
                        .Select(set => new RankedAlleleSetEnvelope(
                            set.GeneId.Value,
                            set.Entries
                                .OrderBy(entry => entry.Rank)
                                .ThenBy(entry => entry.AlleleId)
                                .Select(entry => new RankedAlleleEntryEnvelope(
                                    entry.AlleleId.Value,
                                    entry.Rank,
                                    entry.NumericValue))
                                .ToArray()))
                        .ToArray()))
                .ToArray());
    }

    private static GenomeVersion FromEnvelope(
        GenomeVersionEnvelope envelope,
        SystemDefinitionVersion expectedSystemDefinitionVersion,
        Func<SystemDefinitionVersion, SystemDefinitionVersion, GenomeCompatibilityResult>? compatibility)
    {
        if (envelope.EnvelopeVersion != EnvelopeVersion)
        {
            throw new GenomeSerializationException($"Unsupported genome JSON envelope version '{envelope.EnvelopeVersion}'.");
        }

        var actualVersion = SystemDefinitionVersion.Parse(Required(envelope.SystemDefinitionVersion, "systemDefinitionVersion"));
        var compatibilityResult = compatibility?.Invoke(actualVersion, expectedSystemDefinitionVersion)
            ?? (actualVersion == expectedSystemDefinitionVersion
                ? GenomeCompatibilityResult.Compatible()
                : GenomeCompatibilityResult.Incompatible(actualVersion, expectedSystemDefinitionVersion));

        if (!compatibilityResult.IsCompatible)
        {
            throw new GenomeSerializationException(compatibilityResult.Message);
        }

        return new GenomeVersion(
            GenomeVersionId.Parse(Required(envelope.Id, "id")),
            actualVersion,
            ExternalIndividualId.Parse(Required(envelope.IndividualId, "individualId")),
            new GenomeState((envelope.Groups ?? []).Select(group => new GenomeGroupState(
                ResourceId.Parse(Required(group.GroupId, "groupId")),
                (group.GeneAlleles ?? []).Select(set => new RankedAlleleSet(
                    ResourceId.Parse(Required(set.GeneId, "geneId")),
                    (set.Entries ?? []).Select(entry => new RankedAlleleEntry(
                        ResourceId.Parse(Required(entry.AlleleId, "alleleId")),
                        entry.Rank,
                        entry.NumericValue))))))),
            string.IsNullOrWhiteSpace(envelope.ParentVersionId) ? null : GenomeVersionId.Parse(envelope.ParentVersionId),
            envelope.ChangeSummary ?? string.Empty);
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException($"Genome JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record GenomeVersionEnvelope(
        int EnvelopeVersion,
        string? Id,
        string? SystemDefinitionVersion,
        string? IndividualId,
        string? ParentVersionId,
        string? ChangeSummary,
        IReadOnlyList<GenomeGroupEnvelope>? Groups);

    private sealed record GenomeGroupEnvelope(
        string? GroupId,
        IReadOnlyList<RankedAlleleSetEnvelope>? GeneAlleles);

    private sealed record RankedAlleleSetEnvelope(
        string? GeneId,
        IReadOnlyList<RankedAlleleEntryEnvelope>? Entries);

    private sealed record RankedAlleleEntryEnvelope(
        string? AlleleId,
        int Rank,
        double? NumericValue);
}
