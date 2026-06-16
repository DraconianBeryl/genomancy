using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Templates;

public static class PopulationTemplateGroupBinaryCodec
{
    private static readonly byte[] Magic = "GNOMTGP1"u8.ToArray();

    public static void Write(Stream stream, PopulationTemplateGroupVersion group)
    {
        ArgumentNullException.ThrowIfNull(group);

        JsonBackedBinaryEnvelopeCodec.Write(
            stream,
            Magic,
            PopulationTemplateGroupJsonCodec.WriteToBuffer(group));
    }

    public static byte[] WriteToBuffer(PopulationTemplateGroupVersion group)
    {
        ArgumentNullException.ThrowIfNull(group);

        return JsonBackedBinaryEnvelopeCodec.WriteToBuffer(
            Magic,
            PopulationTemplateGroupJsonCodec.WriteToBuffer(group));
    }

    public static PopulationTemplateGroupVersion Read(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = JsonBackedBinaryEnvelopeCodec.ReadJson(stream, Magic, "Population template group");
        return PopulationTemplateGroupJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }

    public static PopulationTemplateGroupVersion ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = JsonBackedBinaryEnvelopeCodec.ReadJsonFromBuffer(buffer, Magic, "Population template group");
        return PopulationTemplateGroupJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }
}
