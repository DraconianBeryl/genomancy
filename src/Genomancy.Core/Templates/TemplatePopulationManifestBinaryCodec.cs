using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Templates;

public static class TemplatePopulationManifestBinaryCodec
{
    private static readonly byte[] Magic = "GNOMTPM1"u8.ToArray();

    public static void Write(Stream stream, TemplatePopulationManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        JsonBackedBinaryEnvelopeCodec.Write(
            stream,
            Magic,
            TemplatePopulationManifestJsonCodec.WriteToBuffer(manifest));
    }

    public static byte[] WriteToBuffer(TemplatePopulationManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        return JsonBackedBinaryEnvelopeCodec.WriteToBuffer(
            Magic,
            TemplatePopulationManifestJsonCodec.WriteToBuffer(manifest));
    }

    public static TemplatePopulationManifest Read(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = JsonBackedBinaryEnvelopeCodec.ReadJson(stream, Magic, "Template population manifest");
        return TemplatePopulationManifestJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }

    public static TemplatePopulationManifest ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = JsonBackedBinaryEnvelopeCodec.ReadJsonFromBuffer(buffer, Magic, "Template population manifest");
        return TemplatePopulationManifestJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }
}
