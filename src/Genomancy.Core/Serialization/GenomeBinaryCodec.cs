using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Serialization;

public static class GenomeBinaryCodec
{
    private static readonly byte[] Magic = "GNOMGEN1"u8.ToArray();

    public static void WriteVersion(Stream stream, GenomeVersion version)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(version);

        stream.Write(Magic);
        var json = GenomeJsonCodec.WriteVersionToBuffer(version);
        Span<byte> length = stackalloc byte[4];
        BitConverter.TryWriteBytes(length, json.Length);
        stream.Write(length);
        stream.Write(json);
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
        ArgumentNullException.ThrowIfNull(stream);

        var magic = new byte[Magic.Length];
        ReadExactly(stream, magic);

        if (!magic.SequenceEqual(Magic))
        {
            throw new GenomeSerializationException("Genome binary payload has an invalid header.");
        }

        Span<byte> lengthBytes = stackalloc byte[4];
        ReadExactly(stream, lengthBytes);
        var length = BitConverter.ToInt32(lengthBytes);

        if (length < 0)
        {
            throw new GenomeSerializationException("Genome binary payload has an invalid length.");
        }

        var json = new byte[length];
        ReadExactly(stream, json);

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

    private static void ReadExactly(Stream stream, Span<byte> buffer)
    {
        var totalRead = 0;

        while (totalRead < buffer.Length)
        {
            var read = stream.Read(buffer[totalRead..]);

            if (read == 0)
            {
                throw new GenomeSerializationException("Genome binary payload ended unexpectedly.");
            }

            totalRead += read;
        }
    }
}
