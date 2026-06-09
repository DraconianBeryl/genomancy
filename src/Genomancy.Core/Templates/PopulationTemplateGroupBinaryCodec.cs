using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Templates;

public static class PopulationTemplateGroupBinaryCodec
{
    private static readonly byte[] Magic = "GNOMTPG1"u8.ToArray();

    public static void Write(Stream stream, PopulationTemplateGroupVersion group)
    {
        ArgumentNullException.ThrowIfNull(group);

        BinaryEnvelopeCodec.Write(stream, Magic, PopulationTemplateGroupJsonCodec.WriteToBuffer(group));
    }

    public static byte[] WriteToBuffer(PopulationTemplateGroupVersion group)
    {
        using var stream = new MemoryStream();
        Write(stream, group);
        return stream.ToArray();
    }

    public static PopulationTemplateGroupVersion Read(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = BinaryEnvelopeCodec.Read(stream, Magic, "Population template group");
        return PopulationTemplateGroupJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }

    public static PopulationTemplateGroupVersion ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream, expectedSystemDefinitionVersion);
    }
}
