using System.Text.Json;
using System.Text.Json.Serialization;
using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Simulation;

public static class ReproducibilityPacketJsonCodec
{
    private const int EnvelopeVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public static string WriteToText(ReproducibilityPacket packet)
    {
        ArgumentNullException.ThrowIfNull(packet);
        return JsonSerializer.Serialize(ToEnvelope(packet), JsonOptions);
    }

    public static byte[] WriteToBuffer(ReproducibilityPacket packet)
    {
        ArgumentNullException.ThrowIfNull(packet);
        return JsonSerializer.SerializeToUtf8Bytes(ToEnvelope(packet), JsonOptions);
    }

    public static void Write(Stream stream, ReproducibilityPacket packet)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(packet);
        JsonSerializer.Serialize(stream, ToEnvelope(packet), JsonOptions);
    }

    public static ReproducibilityPacket Read(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        try
        {
            var envelope = JsonSerializer.Deserialize<PacketEnvelope>(stream, JsonOptions)
                ?? throw new GenomeSerializationException("Reproducibility packet JSON envelope was empty.");

            if (envelope.EnvelopeVersion != EnvelopeVersion)
            {
                throw new GenomeSerializationException(
                    $"Unsupported reproducibility packet envelope version '{envelope.EnvelopeVersion}'.");
            }

            return new ReproducibilityPacket(
                SystemDefinitionVersion.Parse(Required(envelope.ResourceSetVersion, "resourceSetVersion")),
                Required(envelope.TestIdentifier, "testIdentifier"),
                envelope.Seed,
                Required(envelope.OperationPath, "operationPath"),
                Required(envelope.InputState, "inputState"),
                Required(envelope.FailureAssertion, "failureAssertion"),
                envelope.ExpectedValue,
                envelope.ActualValue,
                Required(envelope.Diagnostic, "diagnostic"));
        }
        catch (JsonException exception)
        {
            throw new GenomeSerializationException("Reproducibility packet JSON is malformed.", exception);
        }
    }

    public static ReproducibilityPacket ReadFromBuffer(ReadOnlySpan<byte> buffer)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream);
    }

    private static PacketEnvelope ToEnvelope(ReproducibilityPacket packet)
    {
        return new PacketEnvelope(
            EnvelopeVersion,
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

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException(
                $"Reproducibility packet JSON property '{propertyName}' is required.");
        }

        return value;
    }

    private sealed record PacketEnvelope(
        int EnvelopeVersion,
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
