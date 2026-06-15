using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestBatchRunResultJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static void Write(Stream stream, ResourceTestBatchRunResult result)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(result);

        JsonSerializer.Serialize(stream, ToEnvelope(result), JsonOptions);
    }

    public static string WriteToText(ResourceTestBatchRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return JsonSerializer.Serialize(ToEnvelope(result), JsonOptions);
    }

    public static byte[] WriteToBuffer(ResourceTestBatchRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(result), JsonOptions);
    }

    public static ResourceTestBatchRunResult Read(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var envelope = JsonSerializer.Deserialize<BatchResultEnvelope>(stream, JsonOptions)
                ?? throw new GenomeSerializationException("Resource test batch result JSON envelope was empty.");

            return FromEnvelope(envelope);
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Resource test batch result JSON is malformed.", exception);
        }
        catch (ArgumentException exception)
        {
            throw new GenomeSerializationException("Resource test batch result JSON is invalid.", exception);
        }
    }

    public static ResourceTestBatchRunResult ReadFromBuffer(ReadOnlySpan<byte> buffer)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream);
    }

    private static BatchResultEnvelope ToEnvelope(ResourceTestBatchRunResult result)
    {
        return new BatchResultEnvelope(
            EnvelopeVersion,
            result.Status.ToString(),
            result.Runs
                .OrderBy(run => run.RunId)
                .ThenBy(run => run.ResultPath, StringComparer.Ordinal)
                .Select(run => new BatchRunResultEnvelope(
                    run.RunId.Value,
                    run.ResultPath,
                    run.ManifestEntry.CompletedAtUtc?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                    run.ManifestEntry.Label,
                    run.ManifestEntry.Tags.Order(StringComparer.Ordinal).ToArray(),
                    ResourceTestResultElement(run.Result)))
                .ToArray());
    }

    private static ResourceTestBatchRunResult FromEnvelope(BatchResultEnvelope envelope)
    {
        if (envelope.EnvelopeVersion != EnvelopeVersion)
        {
            throw new GenomeSerializationException(
                $"Unsupported resource test batch result envelope version '{envelope.EnvelopeVersion}'.");
        }

        var result = new ResourceTestBatchRunResult((envelope.Runs ?? []).Select(run =>
        {
            var runId = ResourceTestId.Parse(Required(run.RunId, "run.runId"));
            var resultPath = Required(run.ResultPath, "run.resultPath");
            var runResult = ReadResourceTestResult(run.Result);
            var manifestEntry = ResourceTestResultManifestEntry.FromResult(
                runId,
                resultPath,
                runResult,
                ParseCompletedAtUtc(run.CompletedAtUtc),
                run.Label,
                run.Tags ?? []);

            return new ResourceTestBatchRunRecord(
                runId,
                resultPath,
                runResult,
                manifestEntry);
        }));

        var expectedStatus = ParseEnum<ResourceTestStatus>(Required(envelope.Status, "status"), "status");

        if (result.Status != expectedStatus)
        {
            throw new GenomeSerializationException(
                $"Resource test batch result status '{expectedStatus}' does not match run-derived status '{result.Status}'.");
        }

        return result;
    }

    private static JsonElement ResourceTestResultElement(ResourceTestRunResult result)
    {
        using var document = JsonDocument.Parse(ResourceTestResultJsonCodec.WriteToText(result));
        return document.RootElement.Clone();
    }

    private static ResourceTestRunResult ReadResourceTestResult(JsonElement result)
    {
        if (result.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            throw new GenomeSerializationException("Resource test batch result JSON property 'run.result' is required.");
        }

        return ResourceTestResultJsonCodec.ReadFromBuffer(Encoding.UTF8.GetBytes(result.GetRawText()));
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
                $"Resource test batch result JSON property 'run.completedAtUtc' has unsupported value '{value}'.");
        }

        return parsed.ToUniversalTime();
    }

    private static TEnum ParseEnum<TEnum>(string value, string propertyName)
        where TEnum : struct
    {
        if (!Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed))
        {
            throw new GenomeSerializationException(
                $"Resource test batch result JSON property '{propertyName}' has unsupported value '{value}'.");
        }

        return parsed;
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException(
                $"Resource test batch result JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record BatchResultEnvelope(
        int EnvelopeVersion,
        string? Status,
        IReadOnlyList<BatchRunResultEnvelope>? Runs);

    private sealed record BatchRunResultEnvelope(
        string? RunId,
        string? ResultPath,
        string? CompletedAtUtc,
        string? Label,
        IReadOnlyList<string>? Tags,
        JsonElement Result);
}
