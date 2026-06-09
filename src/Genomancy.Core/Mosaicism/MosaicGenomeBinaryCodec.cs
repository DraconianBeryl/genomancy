using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Mosaicism;

public static class MosaicGenomeBinaryCodec
{
    private static readonly byte[] Magic = "GNOMMOS1"u8.ToArray();

    public static void Write(Stream stream, MosaicGenomeState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        BinaryEnvelopeCodec.Write(stream, Magic, MosaicGenomeJsonCodec.WriteToBuffer(state));
    }

    public static byte[] WriteToBuffer(MosaicGenomeState state)
    {
        using var stream = new MemoryStream();
        Write(stream, state);
        return stream.ToArray();
    }

    public static MosaicGenomeState Read(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = BinaryEnvelopeCodec.Read(stream, Magic, "Mosaic genome");
        return MosaicGenomeJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }

    public static MosaicGenomeState ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream, expectedSystemDefinitionVersion);
    }
}
