using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Variants;

public static class RuntimeBodyPlanVariantBinaryCodec
{
    private static readonly byte[] Magic = "GNOMVAR1"u8.ToArray();

    public static void Write(Stream stream, RuntimeBodyPlanVariant variant)
    {
        ArgumentNullException.ThrowIfNull(variant);

        JsonBackedBinaryEnvelopeCodec.Write(
            stream,
            Magic,
            RuntimeBodyPlanVariantJsonCodec.WriteToBuffer(variant));
    }

    public static byte[] WriteToBuffer(RuntimeBodyPlanVariant variant)
    {
        ArgumentNullException.ThrowIfNull(variant);

        return JsonBackedBinaryEnvelopeCodec.WriteToBuffer(
            Magic,
            RuntimeBodyPlanVariantJsonCodec.WriteToBuffer(variant));
    }

    public static RuntimeBodyPlanVariant Read(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = JsonBackedBinaryEnvelopeCodec.ReadJson(stream, Magic, "Body-plan variant");
        return RuntimeBodyPlanVariantJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }

    public static RuntimeBodyPlanVariant ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = JsonBackedBinaryEnvelopeCodec.ReadJsonFromBuffer(buffer, Magic, "Body-plan variant");
        return RuntimeBodyPlanVariantJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }
}
