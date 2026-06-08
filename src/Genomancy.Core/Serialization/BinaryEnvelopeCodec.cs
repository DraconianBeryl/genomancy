namespace Genomancy.Core.Serialization;

internal static class BinaryEnvelopeCodec
{
    public static void Write(Stream stream, ReadOnlySpan<byte> magic, ReadOnlySpan<byte> payload)
    {
        ArgumentNullException.ThrowIfNull(stream);

        stream.Write(magic);
        Span<byte> length = stackalloc byte[4];
        BitConverter.TryWriteBytes(length, payload.Length);
        stream.Write(length);
        stream.Write(payload);
    }

    public static byte[] Read(Stream stream, ReadOnlySpan<byte> magic, string payloadName)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var actualMagic = new byte[magic.Length];
        ReadExactly(stream, actualMagic, payloadName);

        if (!actualMagic.AsSpan().SequenceEqual(magic))
        {
            throw new GenomeSerializationException($"{payloadName} binary payload has an invalid header.");
        }

        Span<byte> lengthBytes = stackalloc byte[4];
        ReadExactly(stream, lengthBytes, payloadName);
        var length = BitConverter.ToInt32(lengthBytes);

        if (length < 0)
        {
            throw new GenomeSerializationException($"{payloadName} binary payload has an invalid length.");
        }

        var payload = new byte[length];
        ReadExactly(stream, payload, payloadName);
        return payload;
    }

    private static void ReadExactly(Stream stream, Span<byte> buffer, string payloadName)
    {
        var totalRead = 0;

        while (totalRead < buffer.Length)
        {
            var read = stream.Read(buffer[totalRead..]);

            if (read == 0)
            {
                throw new GenomeSerializationException($"{payloadName} binary payload ended unexpectedly.");
            }

            totalRead += read;
        }
    }
}
