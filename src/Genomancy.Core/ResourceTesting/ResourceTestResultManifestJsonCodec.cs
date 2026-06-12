using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestResultManifestJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static void Write(Stream stream, ResourceTestResultManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(manifest);

        JsonSerializer.Serialize(stream, ToEnvelope(manifest), JsonOptions);
    }

    public static string WriteToText(ResourceTestResultManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        return JsonSerializer.Serialize(ToEnvelope(manifest), JsonOptions);
    }

    public static byte[] WriteToBuffer(ResourceTestResultManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(manifest), JsonOptions);
    }

    public static ResourceTestResultManifest Read(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var envelope = JsonSerializer.Deserialize<ManifestEnvelope>(stream, JsonOptions)
                ?? throw new GenomeSerializationException("Resource test result manifest JSON envelope was empty.");

            return FromEnvelope(envelope);
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Resource test result manifest JSON is malformed.", exception);
        }
        catch (ArgumentException exception)
        {
            throw new GenomeSerializationException("Resource test result manifest JSON is invalid.", exception);
        }
    }

    public static ResourceTestResultManifest ReadFromBuffer(ReadOnlySpan<byte> buffer)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream);
    }

    private static ManifestEnvelope ToEnvelope(ResourceTestResultManifest manifest)
    {
        return new ManifestEnvelope(
            EnvelopeVersion,
            manifest.Entries
                .OrderBy(entry => entry.RunId)
                .ThenBy(entry => entry.ResultPath, StringComparer.Ordinal)
                .Select(entry => new ManifestEntryEnvelope(
                    entry.RunId.Value,
                    entry.ResultPath,
                    entry.CompletedAtUtc?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                    entry.Label,
                    entry.Tags.Order(StringComparer.Ordinal).ToArray(),
                    ToSummaryEnvelope(entry.Summary)))
                .ToArray());
    }

    private static ResourceTestResultManifest FromEnvelope(ManifestEnvelope envelope)
    {
        if (envelope.EnvelopeVersion != EnvelopeVersion)
        {
            throw new GenomeSerializationException(
                $"Unsupported resource test result manifest envelope version '{envelope.EnvelopeVersion}'.");
        }

        return new ResourceTestResultManifest((envelope.Entries ?? []).Select(entry =>
            new ResourceTestResultManifestEntry(
                ResourceTestId.Parse(Required(entry.RunId, "entry.runId")),
                Required(entry.ResultPath, "entry.resultPath"),
                FromSummaryEnvelope(entry.Summary ?? throw new GenomeSerializationException(
                    "Resource test result manifest JSON property 'entry.summary' is required.")),
                ParseCompletedAtUtc(entry.CompletedAtUtc),
                entry.Label,
                entry.Tags ?? [])));
    }

    private static SummaryEnvelope ToSummaryEnvelope(ResourceTestRunSummary summary)
    {
        return new SummaryEnvelope(
            summary.Status.ToString(),
            summary.TotalCases,
            summary.PassedCases,
            summary.FailedCases,
            summary.TotalDiagnostics,
            summary.ErrorDiagnostics,
            summary.WarningDiagnostics,
            summary.InfoDiagnostics,
            summary.ReproducibilityPackets);
    }

    private static ResourceTestRunSummary FromSummaryEnvelope(SummaryEnvelope summary)
    {
        return new ResourceTestRunSummary(
            ParseEnum<ResourceTestStatus>(Required(summary.Status, "summary.status"), "summary.status"),
            summary.TotalCases,
            summary.PassedCases,
            summary.FailedCases,
            summary.TotalDiagnostics,
            summary.ErrorDiagnostics,
            summary.WarningDiagnostics,
            summary.InfoDiagnostics,
            summary.ReproducibilityPackets);
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
                $"Resource test result manifest JSON property 'entry.completedAtUtc' has unsupported value '{value}'.");
        }

        return parsed.ToUniversalTime();
    }

    private static TEnum ParseEnum<TEnum>(string value, string propertyName)
        where TEnum : struct
    {
        if (!Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed))
        {
            throw new GenomeSerializationException(
                $"Resource test result manifest JSON property '{propertyName}' has unsupported value '{value}'.");
        }

        return parsed;
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException(
                $"Resource test result manifest JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record ManifestEnvelope(
        int EnvelopeVersion,
        IReadOnlyList<ManifestEntryEnvelope>? Entries);

    private sealed record ManifestEntryEnvelope(
        string? RunId,
        string? ResultPath,
        string? CompletedAtUtc,
        string? Label,
        IReadOnlyList<string>? Tags,
        SummaryEnvelope? Summary);

    private sealed record SummaryEnvelope(
        string? Status,
        int TotalCases,
        int PassedCases,
        int FailedCases,
        int TotalDiagnostics,
        int ErrorDiagnostics,
        int WarningDiagnostics,
        int InfoDiagnostics,
        int ReproducibilityPackets);
}
