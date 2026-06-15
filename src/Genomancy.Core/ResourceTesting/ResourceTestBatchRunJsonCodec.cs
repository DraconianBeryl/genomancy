using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestBatchRunJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static void Write(Stream stream, IEnumerable<ResourceTestBatchRunSpecification> runs)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(runs);

        JsonSerializer.Serialize(stream, ToEnvelope(runs), JsonOptions);
    }

    public static string WriteToText(IEnumerable<ResourceTestBatchRunSpecification> runs)
    {
        ArgumentNullException.ThrowIfNull(runs);

        return JsonSerializer.Serialize(ToEnvelope(runs), JsonOptions);
    }

    public static byte[] WriteToBuffer(IEnumerable<ResourceTestBatchRunSpecification> runs)
    {
        ArgumentNullException.ThrowIfNull(runs);

        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(runs), JsonOptions);
    }

    public static IReadOnlyList<ResourceTestBatchRunSpecification> Read(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var envelope = JsonSerializer.Deserialize<BatchEnvelope>(stream, JsonOptions)
                ?? throw new GenomeSerializationException("Resource test batch JSON envelope was empty.");

            return FromEnvelope(envelope);
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Resource test batch JSON is malformed.", exception);
        }
        catch (ArgumentException exception)
        {
            throw new GenomeSerializationException("Resource test batch JSON is invalid.", exception);
        }
    }

    public static IReadOnlyList<ResourceTestBatchRunSpecification> ReadFromBuffer(ReadOnlySpan<byte> buffer)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream);
    }

    public static IReadOnlyList<ResourceTestBatchRunRequest> ReadRequestsFromBuffer(ReadOnlySpan<byte> buffer)
    {
        return ReadFromBuffer(buffer)
            .Select(specification => specification.ToRequest())
            .ToArray();
    }

    private static BatchEnvelope ToEnvelope(IEnumerable<ResourceTestBatchRunSpecification> runs)
    {
        return new BatchEnvelope(
            EnvelopeVersion,
            runs
                .OrderBy(run => run.RunId)
                .ThenBy(run => run.ResultPath, StringComparer.Ordinal)
                .Select(ToRunEnvelope)
                .ToArray());
    }

    private static BatchRunEnvelope ToRunEnvelope(ResourceTestBatchRunSpecification run)
    {
        return new BatchRunEnvelope(
            run.RunId.Value,
            run.ResultPath,
            run.CompletedAtUtc?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            run.Label,
            run.Tags.Order(StringComparer.Ordinal).ToArray(),
            new RunOptionsEnvelope(
                run.Options.IncludeTags.Order(StringComparer.Ordinal).ToArray(),
                run.Options.ExcludeTags.Order(StringComparer.Ordinal).ToArray(),
                run.Options.MaximumDiagnosticSeverity?.ToString()),
            ResourceTestsElement(run.Tests));
    }

    private static IReadOnlyList<ResourceTestBatchRunSpecification> FromEnvelope(BatchEnvelope envelope)
    {
        if (envelope.EnvelopeVersion != EnvelopeVersion)
        {
            throw new GenomeSerializationException(
                $"Unsupported resource test batch envelope version '{envelope.EnvelopeVersion}'.");
        }

        return (envelope.Runs ?? [])
            .Select(run => new ResourceTestBatchRunSpecification(
                ResourceTestId.Parse(Required(run.RunId, "run.runId")),
                Required(run.ResultPath, "run.resultPath"),
                ReadResourceTests(run.ResourceTests),
                FromOptionsEnvelope(run.Options),
                ParseCompletedAtUtc(run.CompletedAtUtc),
                run.Label,
                run.Tags ?? []))
            .OrderBy(run => run.RunId)
            .ThenBy(run => run.ResultPath, StringComparer.Ordinal)
            .ToArray();
    }

    private static JsonElement ResourceTestsElement(IReadOnlyList<ResourceTestSpecification> tests)
    {
        using var document = JsonDocument.Parse(ResourceTestJsonCodec.WriteToText(tests));
        return document.RootElement.Clone();
    }

    private static IReadOnlyList<ResourceTestSpecification> ReadResourceTests(JsonElement resourceTests)
    {
        if (resourceTests.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            throw new GenomeSerializationException("Resource test batch JSON property 'run.resourceTests' is required.");
        }

        return ResourceTestJsonCodec.ReadFromBuffer(Encoding.UTF8.GetBytes(resourceTests.GetRawText()));
    }

    private static ResourceTestRunOptions FromOptionsEnvelope(RunOptionsEnvelope? options)
    {
        return options is null
            ? new ResourceTestRunOptions()
            : new ResourceTestRunOptions(
                options.IncludeTags ?? [],
                options.ExcludeTags ?? [],
                ParseNullableEnum<ResourceTestSeverity>(options.MaximumDiagnosticSeverity, "options.maximumDiagnosticSeverity"));
    }

    private static DateTimeOffset? ParseCompletedAtUtc(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!DateTimeOffset.TryParse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var parsed))
        {
            throw new GenomeSerializationException(
                $"Resource test batch JSON property 'run.completedAtUtc' has unsupported value '{value}'.");
        }

        return parsed.ToUniversalTime();
    }

    private static TEnum? ParseNullableEnum<TEnum>(string? value, string propertyName)
        where TEnum : struct
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed))
        {
            throw new GenomeSerializationException(
                $"Resource test batch JSON property '{propertyName}' has unsupported value '{value}'.");
        }

        return parsed;
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException(
                $"Resource test batch JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record BatchEnvelope(
        int EnvelopeVersion,
        IReadOnlyList<BatchRunEnvelope>? Runs);

    private sealed record BatchRunEnvelope(
        string? RunId,
        string? ResultPath,
        string? CompletedAtUtc,
        string? Label,
        IReadOnlyList<string>? Tags,
        RunOptionsEnvelope? Options,
        JsonElement ResourceTests);

    private sealed record RunOptionsEnvelope(
        IReadOnlyList<string>? IncludeTags,
        IReadOnlyList<string>? ExcludeTags,
        string? MaximumDiagnosticSeverity);
}
