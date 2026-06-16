using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Templates;

public static class TemplatePopulationManifestJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static string WriteToText(TemplatePopulationManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        return JsonSerializer.Serialize(ToEnvelope(manifest), JsonOptions);
    }

    public static byte[] WriteToBuffer(TemplatePopulationManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(manifest), JsonOptions);
    }

    public static TemplatePopulationManifest ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        try
        {
            var envelope = JsonSerializer.Deserialize<ManifestEnvelope>(buffer, JsonOptions)
                ?? throw new GenomeSerializationException("Template population manifest JSON envelope was empty.");

            if (envelope.EnvelopeVersion != EnvelopeVersion)
            {
                throw new GenomeSerializationException($"Unsupported template population manifest envelope version '{envelope.EnvelopeVersion}'.");
            }

            var actualVersion = SystemDefinitionVersion.Parse(Required(envelope.SystemDefinitionVersion, "systemDefinitionVersion"));

            if (actualVersion != expectedSystemDefinitionVersion)
            {
                throw new GenomeSerializationException(
                    $"Template population manifest requires system definition version '{actualVersion}', but '{expectedSystemDefinitionVersion}' was expected.");
            }

            return new TemplatePopulationManifest(
                PopulationTemplateGroupId.Parse(Required(envelope.TemplateGroupId, "templateGroupId")),
                PopulationTemplateGroupVersionId.Parse(Required(envelope.TemplateGroupVersionId, "templateGroupVersionId")),
                actualVersion,
                envelope.Seed,
                envelope.RequestedCount,
                envelope.GenomeVersionPrefix ?? string.Empty,
                envelope.IndividualPrefix ?? string.Empty,
                (envelope.Entries ?? []).Select(entry => new TemplatePopulationManifestEntry(
                    entry.Index,
                    entry.SampleSeed,
                    GenomeVersionId.Parse(Required(entry.GenomeVersionId, "entry.genomeVersionId")),
                    ExternalIndividualId.Parse(Required(entry.IndividualId, "entry.individualId")),
                    (entry.GroupPath ?? []).Select(PopulationTemplateGroupId.Parse),
                    PopulationTemplateId.Parse(Required(entry.PrimaryTemplateId, "entry.primaryTemplateId")),
                    PopulationTemplateVersionId.Parse(Required(entry.PrimaryTemplateVersionId, "entry.primaryTemplateVersionId")),
                    string.IsNullOrWhiteSpace(entry.SecondaryTemplateId) ? null : PopulationTemplateId.Parse(entry.SecondaryTemplateId),
                    string.IsNullOrWhiteSpace(entry.SecondaryTemplateVersionId) ? null : PopulationTemplateVersionId.Parse(entry.SecondaryTemplateVersionId))));
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Template population manifest JSON is malformed.", exception);
        }
    }

    private static ManifestEnvelope ToEnvelope(TemplatePopulationManifest manifest)
    {
        return new ManifestEnvelope(
            EnvelopeVersion,
            manifest.TemplateGroupId.Value,
            manifest.TemplateGroupVersionId.Value,
            manifest.SystemDefinitionVersion.Value,
            manifest.Seed,
            manifest.RequestedCount,
            manifest.GenomeVersionPrefix,
            manifest.IndividualPrefix,
            manifest.Entries
                .OrderBy(entry => entry.Index)
                .Select(entry => new ManifestEntryEnvelope(
                    entry.Index,
                    entry.SampleSeed,
                    entry.GenomeVersionId.Value,
                    entry.IndividualId.Value,
                    entry.GroupPath.Select(group => group.Value).ToArray(),
                    entry.PrimaryTemplateId.Value,
                    entry.PrimaryTemplateVersionId.Value,
                    entry.SecondaryTemplateId?.Value,
                    entry.SecondaryTemplateVersionId?.Value))
                .ToArray());
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException($"Template population manifest JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record ManifestEnvelope(
        int EnvelopeVersion,
        string? TemplateGroupId,
        string? TemplateGroupVersionId,
        string? SystemDefinitionVersion,
        ulong Seed,
        int RequestedCount,
        string? GenomeVersionPrefix,
        string? IndividualPrefix,
        IReadOnlyList<ManifestEntryEnvelope>? Entries);

    private sealed record ManifestEntryEnvelope(
        int Index,
        ulong SampleSeed,
        string? GenomeVersionId,
        string? IndividualId,
        IReadOnlyList<string>? GroupPath,
        string? PrimaryTemplateId,
        string? PrimaryTemplateVersionId,
        string? SecondaryTemplateId,
        string? SecondaryTemplateVersionId);
}
