using Genomancy.Core.ResourceTesting;

namespace Genomancy.Storage.JsonFile;

public static class ResourceTestResultManifestJsonFileStore
{
    public static JsonFileStore<ResourceTestResultManifest> Create()
    {
        return new JsonFileStore<ResourceTestResultManifest>(
            ResourceTestResultManifestJsonCodec.Write,
            ResourceTestResultManifestJsonCodec.Read);
    }
}
