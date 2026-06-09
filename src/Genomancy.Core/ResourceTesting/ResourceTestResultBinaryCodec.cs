using Genomancy.Core.Serialization;

namespace Genomancy.Core.ResourceTesting;

public static class ResourceTestResultBinaryCodec
{
    private static readonly byte[] Magic = "GNOMRTR1"u8.ToArray();

    public static void Write(Stream stream, ResourceTestRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        BinaryEnvelopeCodec.Write(stream, Magic, ResourceTestResultJsonCodec.WriteToBuffer(result));
    }

    public static byte[] WriteToBuffer(ResourceTestRunResult result)
    {
        using var stream = new MemoryStream();
        Write(stream, result);
        return stream.ToArray();
    }

    public static ResourceTestRunResult Read(Stream stream)
    {
        var json = BinaryEnvelopeCodec.Read(stream, Magic, "Resource test result");
        return ResourceTestResultJsonCodec.ReadFromBuffer(json);
    }

    public static ResourceTestRunResult ReadFromBuffer(ReadOnlySpan<byte> buffer)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream);
    }
}
