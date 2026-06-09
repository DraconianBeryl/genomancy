using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;

namespace Genomancy.Core.Variants;

public static class RuntimeBodyPlanVariantBinaryCodec
{
    private static readonly byte[] Magic = "GNOMBPV1"u8.ToArray();

    public static void Write(Stream stream, RuntimeBodyPlanVariant variant)
    {
        ArgumentNullException.ThrowIfNull(variant);

        BinaryEnvelopeCodec.Write(stream, Magic, RuntimeBodyPlanVariantJsonCodec.WriteToBuffer(variant));
    }

    public static byte[] WriteToBuffer(RuntimeBodyPlanVariant variant)
    {
        using var stream = new MemoryStream();
        Write(stream, variant);
        return stream.ToArray();
    }

    public static RuntimeBodyPlanVariant Read(
        Stream stream,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        var json = BinaryEnvelopeCodec.Read(stream, Magic, "Body-plan variant");
        return RuntimeBodyPlanVariantJsonCodec.ReadFromBuffer(json, expectedSystemDefinitionVersion);
    }

    public static RuntimeBodyPlanVariant ReadFromBuffer(
        ReadOnlySpan<byte> buffer,
        SystemDefinitionVersion expectedSystemDefinitionVersion)
    {
        using var stream = new MemoryStream(buffer.ToArray(), writable: false);
        return Read(stream, expectedSystemDefinitionVersion);
    }
}
