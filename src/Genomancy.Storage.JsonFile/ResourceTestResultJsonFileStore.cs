using Genomancy.Core.ResourceTesting;

namespace Genomancy.Storage.JsonFile;

public static class ResourceTestResultJsonFileStore
{
    public static JsonFileStore<ResourceTestRunResult> Create()
    {
        return new JsonFileStore<ResourceTestRunResult>(
            ResourceTestResultJsonCodec.Write,
            ResourceTestResultJsonCodec.Read);
    }
}
