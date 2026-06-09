using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;
using Genomancy.Core.Simulation;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestResultJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static void Write(Stream stream, ResourceTestRunResult result)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(result);

        JsonSerializer.Serialize(stream, ToEnvelope(result), JsonOptions);
    }

    public static string WriteToText(ResourceTestRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return JsonSerializer.Serialize(ToEnvelope(result), JsonOptions);
    }

    public static byte[] WriteToBuffer(ResourceTestRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(result), JsonOptions);
    }

    public static ResourceTestRunResult Read(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var envelope = JsonSerializer.Deserialize<ResultEnvelope>(stream, JsonOptions)
                ?? throw new GenomeSerializationException("Resource test result JSON envelope was empty.");

            return FromEnvelope(envelope);
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Resource test result JSON is malformed.", exception);
        }
    }

    public static ResourceTestRunResult ReadFromBuffer(ReadOnlySpan<byte> buffer)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream);
    }

    private static ResultEnvelope ToEnvelope(ResourceTestRunResult result)
    {
        return new ResultEnvelope(
            EnvelopeVersion,
            result.Status.ToString(),
            result.Cases
                .OrderBy(testCase => testCase.TestId)
                .Select(testCase => new CaseEnvelope(
                    testCase.TestId.Value,
                    testCase.Status.ToString(),
                    testCase.Tags.Order(StringComparer.Ordinal).ToArray(),
                    testCase.Diagnostics
                        .Order()
                        .Select(diagnostic => new DiagnosticEnvelope(
                            diagnostic.Severity.ToString(),
                            diagnostic.Code,
                            diagnostic.Path,
                            diagnostic.Message))
                        .ToArray(),
                    testCase.ReproducibilityPackets
                        .OrderBy(packet => packet.OperationPath, StringComparer.Ordinal)
                        .Select(ToPacketEnvelope)
                        .ToArray()))
                .ToArray());
    }

    private static ResourceTestRunResult FromEnvelope(ResultEnvelope envelope)
    {
        if (envelope.EnvelopeVersion != EnvelopeVersion)
        {
            throw new GenomeSerializationException(
                $"Unsupported resource test result envelope version '{envelope.EnvelopeVersion}'.");
        }

        var result = new ResourceTestRunResult((envelope.Cases ?? []).Select(testCase => new ResourceTestCaseResult(
            ResourceTestId.Parse(Required(testCase.TestId, "case.testId")),
            ParseEnum<ResourceTestStatus>(Required(testCase.Status, "case.status"), "case.status"),
            (testCase.Diagnostics ?? []).Select(diagnostic => new ResourceTestDiagnostic(
                ParseEnum<ResourceTestSeverity>(Required(diagnostic.Severity, "diagnostic.severity"), "diagnostic.severity"),
                Required(diagnostic.Code, "diagnostic.code"),
                Required(diagnostic.Path, "diagnostic.path"),
                Required(diagnostic.Message, "diagnostic.message"))),
            testCase.Tags ?? [],
            (testCase.ReproducibilityPackets ?? []).Select(FromPacketEnvelope))));

        var expectedStatus = ParseEnum<ResourceTestStatus>(Required(envelope.Status, "status"), "status");

        if (result.Status != expectedStatus)
        {
            throw new GenomeSerializationException(
                $"Resource test result status '{expectedStatus}' does not match case-derived status '{result.Status}'.");
        }

        return result;
    }

    private static PacketEnvelope ToPacketEnvelope(ReproducibilityPacket packet)
    {
        return new PacketEnvelope(
            packet.ResourceSetVersion.Value,
            packet.TestIdentifier,
            packet.Seed,
            packet.OperationPath,
            packet.InputState,
            packet.FailureAssertion,
            packet.ExpectedValue,
            packet.ActualValue,
            packet.Diagnostic);
    }

    private static ReproducibilityPacket FromPacketEnvelope(PacketEnvelope packet)
    {
        return new ReproducibilityPacket(
            SystemDefinitionVersion.Parse(Required(packet.ResourceSetVersion, "packet.resourceSetVersion")),
            Required(packet.TestIdentifier, "packet.testIdentifier"),
            packet.Seed,
            Required(packet.OperationPath, "packet.operationPath"),
            Required(packet.InputState, "packet.inputState"),
            Required(packet.FailureAssertion, "packet.failureAssertion"),
            packet.ExpectedValue,
            packet.ActualValue,
            Required(packet.Diagnostic, "packet.diagnostic"));
    }

    private static TEnum ParseEnum<TEnum>(string value, string propertyName)
        where TEnum : struct
    {
        if (!Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed))
        {
            throw new GenomeSerializationException(
                $"Resource test result JSON property '{propertyName}' has unsupported value '{value}'.");
        }

        return parsed;
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException(
                $"Resource test result JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record ResultEnvelope(
        int EnvelopeVersion,
        string? Status,
        IReadOnlyList<CaseEnvelope>? Cases);

    private sealed record CaseEnvelope(
        string? TestId,
        string? Status,
        IReadOnlyList<string>? Tags,
        IReadOnlyList<DiagnosticEnvelope>? Diagnostics,
        IReadOnlyList<PacketEnvelope>? ReproducibilityPackets);

    private sealed record DiagnosticEnvelope(
        string? Severity,
        string? Code,
        string? Path,
        string? Message);

    private sealed record PacketEnvelope(
        string? ResourceSetVersion,
        string? TestIdentifier,
        ulong Seed,
        string? OperationPath,
        string? InputState,
        string? FailureAssertion,
        double ExpectedValue,
        double ActualValue,
        string? Diagnostic);
}
