using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Serialization;

public static class GenomeBinaryCodec
{
    private static readonly byte[] Magic = "GNOMGEN1"u8.ToArray();

    public static void WriteVersion(Stream stream, GenomeVersion version)
    {
        ArgumentNullException.ThrowIfNull(version);

        BinaryEnvelopeCodec.Write(stream, Magic, GenomeJsonCodec.WriteVersionToBuffer(version));
    }

    public static byte[] WriteVersionToBuffer(GenomeVersion version)
    {
        using var stream = new MemoryStream();
        WriteVersion(stream, version);
        return stream.ToArray();
    }

    public static GenomeVersion ReadVersion(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion,
        Func<SystemDefinitionVersion, SystemDefinitionVersion, GenomeCompatibilityResult>? compatibility = null)
    {
        var json = BinaryEnvelopeCodec.Read(stream, Magic, "Genome");
        using var jsonStream = new MemoryStream(json, writable: false);
        return GenomeJsonCodec.ReadVersion(jsonStream, expectedSystemDefinitionVersion, compatibility);
    }

    public static GenomeVersion ReadVersionFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion,
        Func<SystemDefinitionVersion, SystemDefinitionVersion, GenomeCompatibilityResult>? compatibility = null)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return ReadVersion(stream, expectedSystemDefinitionVersion, compatibility);
    }

}
