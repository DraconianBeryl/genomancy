using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Mosaicism;

public static class MosaicGenomeJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static void Write(Stream stream, MosaicGenomeState state)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(state);

        JsonSerializer.Serialize(stream, ToEnvelope(state), JsonOptions);
    }

    public static string WriteToText(MosaicGenomeState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        return JsonSerializer.Serialize(ToEnvelope(state), JsonOptions);
    }

    public static byte[] WriteToBuffer(MosaicGenomeState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(state), JsonOptions);
    }

    public static MosaicGenomeState Read(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var envelope = JsonSerializer.Deserialize<MosaicGenomeEnvelope>(stream, JsonOptions)
                ?? throw new GenomeSerializationException("Mosaic genome JSON envelope was empty.");

            return FromEnvelope(envelope, expectedSystemDefinitionVersion);
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Mosaic genome JSON is malformed.", exception);
        }
    }

    public static MosaicGenomeState ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream, expectedSystemDefinitionVersion);
    }

    private static MosaicGenomeEnvelope ToEnvelope(MosaicGenomeState state)
    {
        return new MosaicGenomeEnvelope(
            EnvelopeVersion,
            GenomeJsonCodec.WriteVersionToText(state.PrimaryGenomeVersion),
            state.Regions
                .OrderBy(region => region.RegionId)
                .Select(region => new RegionEnvelope(
                    region.RegionId.Value,
                    GenomeJsonCodec.WriteVersionToText(region.GenomeVersion),
                    region.Coverage))
                .ToArray(),
            state.ChimericMaterials
                .OrderBy(material => material.Id)
                .Select(material => new ChimericMaterialEnvelope(
                    material.Id.Value,
                    GenomeJsonCodec.WriteVersionToText(material.GenomeVersion),
                    material.ExpressedRegionIds.Select(regionId => regionId.Value).Order(StringComparer.Ordinal).ToArray(),
                    material.IsIntegratedBodyPlanVariant))
                .ToArray());
    }

    private static MosaicGenomeState FromEnvelope(
        MosaicGenomeEnvelope envelope,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        if (envelope.EnvelopeVersion != EnvelopeVersion)
        {
            throw new GenomeSerializationException(
                $"Unsupported mosaic genome envelope version '{envelope.EnvelopeVersion}'.");
        }

        return new MosaicGenomeState(
            ReadGenome(Required(envelope.PrimaryGenomeVersion, "primaryGenomeVersion"), expectedSystemDefinitionVersion),
            (envelope.Regions ?? []).Select(region => new MosaicRegionAssignment(
                MosaicRegionId.Parse(Required(region.RegionId, "region.regionId")),
                ReadGenome(Required(region.GenomeVersion, "region.genomeVersion"), expectedSystemDefinitionVersion),
                region.Coverage)),
            (envelope.ChimericMaterials ?? []).Select(material => new ChimericMaterialState(
                ResourceId.Parse(Required(material.Id, "chimericMaterial.id")),
                ReadGenome(Required(material.GenomeVersion, "chimericMaterial.genomeVersion"), expectedSystemDefinitionVersion),
                (material.ExpressedRegionIds ?? []).Select(MosaicRegionId.Parse),
                material.IsIntegratedBodyPlanVariant)));
    }

    private static GenomeVersion ReadGenome(
        string genomeJson,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        return GenomeJsonCodec.ReadVersionFromBuffer(
            Encoding.UTF8.GetBytes(genomeJson),
            expectedSystemDefinitionVersion);
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException($"Mosaic genome JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record MosaicGenomeEnvelope(
        int EnvelopeVersion,
        string? PrimaryGenomeVersion,
        IReadOnlyList<RegionEnvelope>? Regions,
        IReadOnlyList<ChimericMaterialEnvelope>? ChimericMaterials);

    private sealed record RegionEnvelope(
        string? RegionId,
        string? GenomeVersion,
        double Coverage);

    private sealed record ChimericMaterialEnvelope(
        string? Id,
        string? GenomeVersion,
        IReadOnlyList<string>? ExpressedRegionIds,
        bool IsIntegratedBodyPlanVariant);
}
