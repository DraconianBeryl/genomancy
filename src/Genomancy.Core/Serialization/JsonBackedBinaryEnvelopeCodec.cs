namespace Genomancy.Core.Serialization;

internal static class JsonBackedBinaryEnvelopeCodec
{
    public static void Write(Stream stream, ReadOnlySpan<byte> magic, ReadOnlySpan<byte> json)
    {
        ArgumentNullException.ThrowIfNull(stream);

        stream.Write(magic);

        Span<byte> length = stackalloc byte[4];
        BitConverter.TryWriteBytes(length, json.Length);
        stream.Write(length);
        stream.Write(json);
    }

    public static byte[] WriteToBuffer(ReadOnlySpan<byte> magic, ReadOnlySpan<byte> json)
    {
        using var stream = new MemoryStream();
        Write(stream, magic, json);
        return stream.ToArray();
    }

    public static byte[] ReadJson(Stream stream, ReadOnlySpan<byte> expectedMagic, string payloadName)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var magic = new byte[expectedMagic.Length];
        ReadExactly(stream, magic, payloadName);

        if (!magic.SequenceEqual(expectedMagic.ToArray()))
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

        var json = new byte[length];
        ReadExactly(stream, json, payloadName);
        return json;
    }

    public static byte[] ReadJsonFromBuffer(
        ReadOnlySpan<byte> buffer,
        ReadOnlySpan<byte> expectedMagic,
        string payloadName)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return ReadJson(stream, expectedMagic, payloadName);
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
