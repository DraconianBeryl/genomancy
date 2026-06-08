using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Templates;

public static class PopulationTemplateBinaryCodec
{
    private static readonly byte[] Magic = "GNOMTPL1"u8.ToArray();

    public static void Write(Stream stream, PopulationTemplateVersion template)
    {
        ArgumentNullException.ThrowIfNull(template);

        BinaryEnvelopeCodec.Write(stream, Magic, PopulationTemplateJsonCodec.WriteToBuffer(template));
    }

    public static byte[] WriteToBuffer(PopulationTemplateVersion template)
    {
        using var stream = new MemoryStream();
        Write(stream, template);
        return stream.ToArray();
    }

    public static PopulationTemplateVersion Read(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = BinaryEnvelopeCodec.Read(stream, Magic, "Population template");
        return PopulationTemplateJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }

    public static PopulationTemplateVersion ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream, expectedSystemDefinitionVersion);
    }
}
