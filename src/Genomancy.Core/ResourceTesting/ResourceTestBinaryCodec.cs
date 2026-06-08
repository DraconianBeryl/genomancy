using Genomancy.Core.Serialization;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestBinaryCodec
{
    private static readonly byte[] Magic = "GNOMRTE1"u8.ToArray();

    public static void Write(Stream stream, IEnumerable<ResourceTestSpecification> specifications)
    {
        ArgumentNullException.ThrowIfNull(specifications);

        BinaryEnvelopeCodec.Write(stream, Magic, ResourceTestJsonCodec.WriteToBuffer(specifications));
    }

    public static byte[] WriteToBuffer(IEnumerable<ResourceTestSpecification> specifications)
    {
        using var stream = new MemoryStream();
        Write(stream, specifications);
        return stream.ToArray();
    }

    public static IReadOnlyList<ResourceTestSpecification> Read(Stream stream)
    {
        var json = BinaryEnvelopeCodec.Read(stream, Magic, "Resource test");
        return ResourceTestJsonCodec.ReadFromBuffer(json);
    }

    public static IReadOnlyList<ResourceTestSpecification> ReadFromBuffer(ReadOnlySpan<byte> buffer)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream);
    }
}
