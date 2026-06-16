using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Templates;

public static class PopulationTemplateGroupJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static void Write(Stream stream, PopulationTemplateGroupVersion group)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(group);

        JsonSerializer.Serialize(stream, ToEnvelope(group), JsonOptions);
    }

    public static string WriteToText(PopulationTemplateGroupVersion group)
    {
        ArgumentNullException.ThrowIfNull(group);

        return JsonSerializer.Serialize(ToEnvelope(group), JsonOptions);
    }

    public static byte[] WriteToBuffer(PopulationTemplateGroupVersion group)
    {
        ArgumentNullException.ThrowIfNull(group);

        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(group), JsonOptions);
    }

    public static PopulationTemplateGroupVersion Read(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var envelope = JsonSerializer.Deserialize<TemplateGroupEnvelope>(stream, JsonOptions)
                ?? throw new GenomeSerializationException("Population template group JSON envelope was empty.");

            return FromEnvelope(envelope, expectedSystemDefinitionVersion);
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Population template group JSON is malformed.", exception);
        }
    }

    public static PopulationTemplateGroupVersion ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream, expectedSystemDefinitionVersion);
    }

    private static TemplateGroupEnvelope ToEnvelope(PopulationTemplateGroupVersion group)
    {
        return new TemplateGroupEnvelope(
            EnvelopeVersion,
            group.Id.Value,
            group.VersionId.Value,
            group.SystemDefinitionVersion.Value,
            group.ChangeSummary,
            new BlendPolicyEnvelope(
                group.CrossTemplateBlendPolicy.Rate,
                group.CrossTemplateBlendPolicy.SecondTemplateWeight),
            group.Templates
                .OrderBy(entry => entry.Template.Id)
                .ThenBy(entry => entry.Template.VersionId)
                .Select(entry => new WeightedTemplateEnvelope(
                    entry.Weight,
                    TemplateToEnvelope(entry.Template)))
                .ToArray(),
            group.ChildGroups
                .OrderBy(entry => entry.Group.Id)
                .ThenBy(entry => entry.Group.VersionId)
                .Select(entry => new WeightedTemplateGroupEnvelope(
                    entry.Weight,
                    ToEnvelope(entry.Group)))
                .ToArray());
    }

    private static PopulationTemplateGroupVersion FromEnvelope(
        TemplateGroupEnvelope envelope,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        if (envelope.EnvelopeVersion != EnvelopeVersion)
        {
            throw new GenomeSerializationException($"Unsupported population template group envelope version '{envelope.EnvelopeVersion}'.");
        }

        var actualVersion = SystemDefinitionVersion.Parse(Required(envelope.SystemDefinitionVersion, "systemDefinitionVersion"));

        if (actualVersion != expectedSystemDefinitionVersion)
        {
            throw new GenomeSerializationException(
                $"Population template group requires system definition version '{actualVersion}', but '{expectedSystemDefinitionVersion}' was expected.");
        }

        return new PopulationTemplateGroupVersion(
            PopulationTemplateGroupId.Parse(Required(envelope.Id, "id")),
            PopulationTemplateGroupVersionId.Parse(Required(envelope.VersionId, "versionId")),
            actualVersion,
            (envelope.Templates ?? []).Select(template => new WeightedPopulationTemplate(
                TemplateFromEnvelope(Required(template.Template, "template"), actualVersion),
                template.Weight)),
            (envelope.ChildGroups ?? []).Select(group => new WeightedPopulationTemplateGroup(
                FromEnvelope(Required(group.Group, "group"), actualVersion),
                group.Weight)),
            envelope.CrossTemplateBlendPolicy is null
                ? new CrossTemplateBlendPolicy(0, 0.5)
                : new CrossTemplateBlendPolicy(
                    envelope.CrossTemplateBlendPolicy.Rate,
                    envelope.CrossTemplateBlendPolicy.SecondTemplateWeight),
            envelope.ChangeSummary ?? string.Empty);
    }

    private static TemplateEnvelope TemplateToEnvelope(PopulationTemplateVersion template)
    {
        return new TemplateEnvelope(
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

    private static PopulationTemplateVersion TemplateFromEnvelope(
        TemplateEnvelope envelope,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var actualVersion = SystemDefinitionVersion.Parse(Required(envelope.SystemDefinitionVersion, "template.systemDefinitionVersion"));

        if (actualVersion != expectedSystemDefinitionVersion)
        {
            throw new GenomeSerializationException(
                $"Nested population template requires system definition version '{actualVersion}', but '{expectedSystemDefinitionVersion}' was expected.");
        }

        return new PopulationTemplateVersion(
            PopulationTemplateId.Parse(Required(envelope.Id, "template.id")),
            PopulationTemplateVersionId.Parse(Required(envelope.VersionId, "template.versionId")),
            actualVersion,
            (envelope.Groups ?? []).Select(group => new GroupTemplate(
                ResourceId.Parse(Required(group.GroupId, "template.groupId")),
                (group.Genes ?? []).Select(gene => new GeneTemplate(
                    ResourceId.Parse(Required(gene.GeneId, "template.geneId")),
                    gene.AlleleCount,
                    (gene.Alleles ?? []).Select(allele => new AlleleFrequency(
                        ResourceId.Parse(Required(allele.AlleleId, "template.alleleId")),
                        allele.Weight,
                        allele.NumericValue)))))),
            envelope.ChangeSummary ?? string.Empty);
    }

    private static T Required<T>(T? value, string propertyName)
        where T : class
    {
        if (value is null)
        {
            throw new GenomeSerializationException($"Population template group JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException($"Population template group JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record TemplateGroupEnvelope(
        int EnvelopeVersion,
        string? Id,
        string? VersionId,
        string? SystemDefinitionVersion,
        string? ChangeSummary,
        BlendPolicyEnvelope? CrossTemplateBlendPolicy,
        IReadOnlyList<WeightedTemplateEnvelope>? Templates,
        IReadOnlyList<WeightedTemplateGroupEnvelope>? ChildGroups);

    private sealed record BlendPolicyEnvelope(double Rate, double SecondTemplateWeight);

    private sealed record WeightedTemplateEnvelope(double Weight, TemplateEnvelope? Template);

    private sealed record WeightedTemplateGroupEnvelope(double Weight, TemplateGroupEnvelope? Group);

    private sealed record TemplateEnvelope(
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
