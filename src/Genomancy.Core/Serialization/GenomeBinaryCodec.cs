using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Serialization;

public static class GenomeBinaryCodec
{
    private static readonly byte[] Magic = "GNOMGEN1"u8.ToArray();

    public static void WriteVersion(Stream stream, GenomeVersion version)
    {
        ArgumentNullException.ThrowIfNull(version);

        JsonBackedBinaryEnvelopeCodec.Write(
            stream,
            Magic,
            GenomeJsonCodec.WriteVersionToBuffer(version));
    }

    public static byte[] WriteVersionToBuffer(GenomeVersion version)
    {
        ArgumentNullException.ThrowIfNull(version);

        return JsonBackedBinaryEnvelopeCodec.WriteToBuffer(
            Magic,
            GenomeJsonCodec.WriteVersionToBuffer(version));
    }

    public static GenomeVersion ReadVersion(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion,
        Func<SystemDefinitionVersion, SystemDefinitionVersion, GenomeCompatibilityResult>? compatibility = null)
    {
        var json = JsonBackedBinaryEnvelopeCodec.ReadJson(stream, Magic, "Genome");
        return GenomeJsonCodec.ReadVersionFromBuffer(json, expectedSystemDefinitionVersion, compatibility);
    }

    public static GenomeVersion ReadVersionFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion,
        Func<SystemDefinitionVersion, SystemDefinitionVersion, GenomeCompatibilityResult>? compatibility = null)
    {
        var json = JsonBackedBinaryEnvelopeCodec.ReadJsonFromBuffer(buffer, Magic, "Genome");
        return GenomeJsonCodec.ReadVersionFromBuffer(json, expectedSystemDefinitionVersion, compatibility);
    }
}
