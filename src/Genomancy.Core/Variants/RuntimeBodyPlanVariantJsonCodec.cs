using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Variants;

public static class RuntimeBodyPlanVariantJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static string WriteToText(RuntimeBodyPlanVariant variant)
    {
        ArgumentNullException.ThrowIfNull(variant);

        return JsonSerializer.Serialize(ToEnvelope(variant), JsonOptions);
    }

    public static byte[] WriteToBuffer(RuntimeBodyPlanVariant variant)
    {
        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(variant), JsonOptions);
    }

    public static RuntimeBodyPlanVariant ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        try
        {
            var envelope = JsonSerializer.Deserialize<VariantEnvelope>(buffer, JsonOptions)
                ?? throw new GenomeSerializationException("Body-plan variant JSON envelope was empty.");

            if (envelope.EnvelopeVersion != EnvelopeVersion)
            {
                throw new GenomeSerializationException($"Unsupported body-plan variant envelope version '{envelope.EnvelopeVersion}'.");
            }

            var actualVersion = SystemDefinitionVersion.Parse(Required(envelope.SystemDefinitionVersion, "systemDefinitionVersion"));

            if (actualVersion != expectedSystemDefinitionVersion)
            {
                throw new GenomeSerializationException(
                    $"Body-plan variant requires system definition version '{actualVersion}', but '{expectedSystemDefinitionVersion}' was expected.");
            }

            return new RuntimeBodyPlanVariant(
                BodyPlanVariantId.Parse(Required(envelope.Id, "id")),
                actualVersion,
                ResourceId.Parse(Required(envelope.BaseBodyPlanId, "baseBodyPlanId")),
                (envelope.RequiredGroupIds ?? []).Select(ResourceId.Parse),
                (envelope.OptionalGroupIds ?? []).Select(ResourceId.Parse),
                (envelope.SharedGroupIds ?? []).Select(ResourceId.Parse),
                envelope.ChangeSummary ?? string.Empty);
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Body-plan variant JSON is malformed.", exception);
        }
    }

    private static VariantEnvelope ToEnvelope(RuntimeBodyPlanVariant variant)
    {
        return new VariantEnvelope(
            EnvelopeVersion,
            variant.Id.Value,
            variant.SystemDefinitionVersion.Value,
            variant.BaseBodyPlanId.Value,
            variant.RequiredGroupIds.Select(id => id.Value).ToArray(),
            variant.OptionalGroupIds.Select(id => id.Value).ToArray(),
            variant.SharedGroupIds.Select(id => id.Value).ToArray(),
            variant.ChangeSummary);
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException($"Body-plan variant JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record VariantEnvelope(
        int EnvelopeVersion,
        string? Id,
        string? SystemDefinitionVersion,
        string? BaseBodyPlanId,
        IReadOnlyList<string>? RequiredGroupIds,
        IReadOnlyList<string>? OptionalGroupIds,
        IReadOnlyList<string>? SharedGroupIds,
        string? ChangeSummary);
}
