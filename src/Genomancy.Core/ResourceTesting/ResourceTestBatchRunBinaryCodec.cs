using Genomancy.Core.Serialization;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestBatchRunBinaryCodec
{
    private static readonly byte[] Magic = "GNOMRTB1"u8.ToArray();

    public static void Write(Stream stream, IEnumerable<ResourceTestBatchRunSpecification> runs)
    {
        ArgumentNullException.ThrowIfNull(runs);

        BinaryEnvelopeCodec.Write(stream, Magic, ResourceTestBatchRunJsonCodec.WriteToBuffer(runs));
    }

    public static byte[] WriteToBuffer(IEnumerable<ResourceTestBatchRunSpecification> runs)
    {
        using var stream = new MemoryStream();
        Write(stream, runs);
        return stream.ToArray();
    }

    public static IReadOnlyList<ResourceTestBatchRunSpecification> Read(Stream stream)
    {
        var json = BinaryEnvelopeCodec.Read(stream, Magic, "Resource test batch");
        return ResourceTestBatchRunJsonCodec.ReadFromBuffer(json);
    }

    public static IReadOnlyList<ResourceTestBatchRunSpecification> ReadFromBuffer(ReadOnlySpan<byte> buffer)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream);
    }
}
