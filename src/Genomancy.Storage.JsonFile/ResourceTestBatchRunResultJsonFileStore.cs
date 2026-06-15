using Genomancy.Core.ResourceTesting;

namespace Genomancy.Storage.JsonFile;

public static class ResourceTestBatchRunResultJsonFileStore
{
    public static JsonFileStore<ResourceTestBatchRunResult> Create()
    {
        return new JsonFileStore<ResourceTestBatchRunResult>(
            ResourceTestBatchRunResultJsonCodec.Write,
            ResourceTestBatchRunResultJsonCodec.Read);
    }
}
