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
            var envelope = JsonSerializer.Deserialize<GroupEnvelope>(stream, JsonOptions)
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

    private static GroupEnvelope ToEnvelope(PopulationTemplateGroupVersion group)
    {
        return new GroupEnvelope(
            EnvelopeVersion,
            group.Id.Value,
            group.VersionId.Value,
            group.SystemDefinitionVersion.Value,
            group.ChangeSummary,
            new CrossTemplateBlendPolicyEnvelope(
                group.CrossTemplateBlendPolicy.Rate,
                group.CrossTemplateBlendPolicy.SecondTemplateWeight),
            group.Templates
                .OrderBy(entry => entry.Template.Id)
                .ThenBy(entry => entry.Template.VersionId)
                .Select(entry => new WeightedTemplateEnvelope(
                    ToTemplateEnvelope(entry.Template),
                    entry.Weight))
                .ToArray(),
            group.ChildGroups
                .OrderBy(entry => entry.Group.Id)
                .ThenBy(entry => entry.Group.VersionId)
                .Select(entry => new WeightedGroupEnvelope(
                    ToEnvelope(entry.Group),
                    entry.Weight))
                .ToArray());
    }

    private static PopulationTemplateGroupVersion FromEnvelope(
        GroupEnvelope envelope,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        if (envelope.EnvelopeVersion != EnvelopeVersion)
        {
            throw new GenomeSerializationException(
                $"Unsupported population template group envelope version '{envelope.EnvelopeVersion}'.");
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
            (envelope.Templates ?? []).Select(entry => new WeightedPopulationTemplate(
                FromTemplateEnvelope(entry.Template
                    ?? throw new GenomeSerializationException("Population template group JSON weighted template requires 'template'."),
                    expectedSystemDefinitionVersion),
                entry.Weight)),
            (envelope.ChildGroups ?? []).Select(entry => new WeightedPopulationTemplateGroup(
                FromEnvelope(entry.Group
                    ?? throw new GenomeSerializationException("Population template group JSON weighted child group requires 'group'."),
                    expectedSystemDefinitionVersion),
                entry.Weight)),
            envelope.CrossTemplateBlendPolicy is null
                ? null
                : new CrossTemplateBlendPolicy(
                    envelope.CrossTemplateBlendPolicy.Rate,
                    envelope.CrossTemplateBlendPolicy.SecondTemplateWeight),
            envelope.ChangeSummary ?? string.Empty);
    }

    private static TemplateEnvelope ToTemplateEnvelope(PopulationTemplateVersion template)
    {
        return new TemplateEnvelope(
            template.Id.Value,
            template.VersionId.Value,
            template.SystemDefinitionVersion.Value,
            template.ChangeSummary,
            template.GroupTemplates
                .OrderBy(group => group.GroupId)
                .Select(group => new TemplateGroupEnvelope(
                    group.GroupId.Value,
                    group.GeneTemplates
                        .OrderBy(gene => gene.GeneId)
                        .Select(gene => new TemplateGeneEnvelope(
                            gene.GeneId.Value,
                            gene.AlleleCount,
                            gene.AlleleFrequencies
                                .OrderBy(frequency => frequency.AlleleId)
                                .Select(frequency => new TemplateAlleleEnvelope(
                                    frequency.AlleleId.Value,
                                    frequency.Weight,
                                    frequency.NumericValue))
                                .ToArray()))
                        .ToArray()))
                .ToArray());
    }

    private static PopulationTemplateVersion FromTemplateEnvelope(
        TemplateEnvelope template,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var actualVersion = SystemDefinitionVersion.Parse(Required(template.SystemDefinitionVersion, "template.systemDefinitionVersion"));

        if (actualVersion != expectedSystemDefinitionVersion)
        {
            throw new GenomeSerializationException(
                $"Population template requires system definition version '{actualVersion}', but '{expectedSystemDefinitionVersion}' was expected.");
        }

        return new PopulationTemplateVersion(
            PopulationTemplateId.Parse(Required(template.Id, "template.id")),
            PopulationTemplateVersionId.Parse(Required(template.VersionId, "template.versionId")),
            actualVersion,
            (template.Groups ?? []).Select(group => new GroupTemplate(
                ResourceId.Parse(Required(group.GroupId, "template.groupId")),
                (group.Genes ?? []).Select(gene => new GeneTemplate(
                    ResourceId.Parse(Required(gene.GeneId, "template.geneId")),
                    gene.AlleleCount,
                    (gene.Alleles ?? []).Select(allele => new AlleleFrequency(
                        ResourceId.Parse(Required(allele.AlleleId, "template.alleleId")),
                        allele.Weight,
                        allele.NumericValue)))))),
            template.ChangeSummary ?? string.Empty);
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException(
                $"Population template group JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record GroupEnvelope(
        int EnvelopeVersion,
        string? Id,
        string? VersionId,
        string? SystemDefinitionVersion,
        string? ChangeSummary,
        CrossTemplateBlendPolicyEnvelope? CrossTemplateBlendPolicy,
        IReadOnlyList<WeightedTemplateEnvelope>? Templates,
        IReadOnlyList<WeightedGroupEnvelope>? ChildGroups);

    private sealed record WeightedTemplateEnvelope(
        TemplateEnvelope? Template,
        double Weight);

    private sealed record WeightedGroupEnvelope(
        GroupEnvelope? Group,
        double Weight);

    private sealed record CrossTemplateBlendPolicyEnvelope(
        double Rate,
        double SecondTemplateWeight);

    private sealed record TemplateEnvelope(
        string? Id,
        string? VersionId,
        string? SystemDefinitionVersion,
        string? ChangeSummary,
        IReadOnlyList<TemplateGroupEnvelope>? Groups);

    private sealed record TemplateGroupEnvelope(
        string? GroupId,
        IReadOnlyList<TemplateGeneEnvelope>? Genes);

    private sealed record TemplateGeneEnvelope(
        string? GeneId,
        int AlleleCount,
        IReadOnlyList<TemplateAlleleEnvelope>? Alleles);

    private sealed record TemplateAlleleEnvelope(
        string? AlleleId,
        double Weight,
        double? NumericValue);
}
