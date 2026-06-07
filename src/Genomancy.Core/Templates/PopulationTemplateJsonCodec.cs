using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Templates;

public static class PopulationTemplateJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static void Write(Stream stream, PopulationTemplateVersion template)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(template);

        JsonSerializer.Serialize(stream, ToEnvelope(template), JsonOptions);
    }

    public static string WriteToText(PopulationTemplateVersion template)
    {
        return JsonSerializer.Serialize(ToEnvelope(template), JsonOptions);
    }

    public static byte[] WriteToBuffer(PopulationTemplateVersion template)
    {
        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(template), JsonOptions);
    }

    public static PopulationTemplateVersion Read(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var envelope = JsonSerializer.Deserialize<TemplateEnvelope>(stream, JsonOptions)
                ?? throw new GenomeSerializationException("Population template JSON envelope was empty.");

            return FromEnvelope(envelope, expectedSystemDefinitionVersion);
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Population template JSON is malformed.", exception);
        }
    }

    public static PopulationTemplateVersion ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream, expectedSystemDefinitionVersion);
    }

    private static TemplateEnvelope ToEnvelope(PopulationTemplateVersion template)
    {
        return new TemplateEnvelope(
            EnvelopeVersion,
            template.Id.Value,
            template.VersionId.Value,
            template.SystemDefinitionVersion.Value,
            template.ChangeSummary,
            template.GroupTemplates
                .OrderBy(group => group.GroupId)
                .Select(group => new GroupEnvelope(
                    group.GroupId.Value,
                    group.GeneTemplates
                        .OrderBy(gene => gene.GeneId)
                        .Select(gene => new GeneEnvelope(
                            gene.GeneId.Value,
                            gene.AlleleCount,
                            gene.AlleleFrequencies
                                .OrderBy(frequency => frequency.AlleleId)
                                .Select(frequency => new AlleleEnvelope(
                                    frequency.AlleleId.Value,
                                    frequency.Weight,
                                    frequency.NumericValue))
                                .ToArray()))
                        .ToArray()))
                .ToArray());
    }

    private static PopulationTemplateVersion FromEnvelope(
        TemplateEnvelope envelope,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        if (envelope.EnvelopeVersion != EnvelopeVersion)
        {
            throw new GenomeSerializationException($"Unsupported population template envelope version '{envelope.EnvelopeVersion}'.");
        }

        var actualVersion = SystemDefinitionVersion.Parse(Required(envelope.SystemDefinitionVersion, "systemDefinitionVersion"));

        if (actualVersion != expectedSystemDefinitionVersion)
        {
            throw new GenomeSerializationException(
                $"Population template requires system definition version '{actualVersion}', but '{expectedSystemDefinitionVersion}' was expected.");
        }

        return new PopulationTemplateVersion(
            PopulationTemplateId.Parse(Required(envelope.Id, "id")),
            PopulationTemplateVersionId.Parse(Required(envelope.VersionId, "versionId")),
            actualVersion,
            (envelope.Groups ?? []).Select(group => new GroupTemplate(
                ResourceId.Parse(Required(group.GroupId, "groupId")),
                (group.Genes ?? []).Select(gene => new GeneTemplate(
                    ResourceId.Parse(Required(gene.GeneId, "geneId")),
                    gene.AlleleCount,
                    (gene.Alleles ?? []).Select(allele => new AlleleFrequency(
                        ResourceId.Parse(Required(allele.AlleleId, "alleleId")),
                        allele.Weight,
                        allele.NumericValue)))))),
            envelope.ChangeSummary ?? string.Empty);
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException($"Population template JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record TemplateEnvelope(
        int EnvelopeVersion,
        string? Id,
        string? VersionId,
        string? SystemDefinitionVersion,
        string? ChangeSummary,
        IReadOnlyList<GroupEnvelope>? Groups);

    private sealed record GroupEnvelope(
        string? GroupId,
        IReadOnlyList<GeneEnvelope>? Genes);

    private sealed record GeneEnvelope(
        string? GeneId,
        int AlleleCount,
        IReadOnlyList<AlleleEnvelope>? Alleles);

    private sealed record AlleleEnvelope(
        string? AlleleId,
        double Weight,
        double? NumericValue);
}
