using Genomancy.Core.Serialization;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestBatchRunResultBinaryCodec
{
    private static readonly byte[] Magic = "GNOMRBR1"u8.ToArray();

    public static void Write(Stream stream, ResourceTestBatchRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        BinaryEnvelopeCodec.Write(stream, Magic, ResourceTestBatchRunResultJsonCodec.WriteToBuffer(result));
    }

    public static byte[] WriteToBuffer(ResourceTestBatchRunResult result)
    {
        using var stream = new MemoryStream();
        Write(stream, result);
        return stream.ToArray();
    }

    public static ResourceTestBatchRunResult Read(Stream stream)
    {
        var json = BinaryEnvelopeCodec.Read(stream, Magic, "Resource test batch result");
        return ResourceTestBatchRunResultJsonCodec.ReadFromBuffer(json);
    }

    public static ResourceTestBatchRunResult ReadFromBuffer(ReadOnlySpan<byte> buffer)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream);
    }
}
