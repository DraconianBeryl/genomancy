using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Templates;

public static class PopulationTemplateBinaryCodec
{
    private static readonly byte[] Magic = "GNOMTPL1"u8.ToArray();

    public static void Write(Stream stream, PopulationTemplateVersion template)
    {
        ArgumentNullException.ThrowIfNull(template);

        JsonBackedBinaryEnvelopeCodec.Write(
            stream,
            Magic,
            PopulationTemplateJsonCodec.WriteToBuffer(template));
    }

    public static byte[] WriteToBuffer(PopulationTemplateVersion template)
    {
        ArgumentNullException.ThrowIfNull(template);

        return JsonBackedBinaryEnvelopeCodec.WriteToBuffer(
            Magic,
            PopulationTemplateJsonCodec.WriteToBuffer(template));
    }

    public static PopulationTemplateVersion Read(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = JsonBackedBinaryEnvelopeCodec.ReadJson(stream, Magic, "Population template");
        return PopulationTemplateJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }

    public static PopulationTemplateVersion ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = JsonBackedBinaryEnvelopeCodec.ReadJsonFromBuffer(buffer, Magic, "Population template");
        return PopulationTemplateJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }
}
