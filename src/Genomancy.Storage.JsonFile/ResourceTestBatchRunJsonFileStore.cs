using Genomancy.Core.ResourceTesting;

namespace Genomancy.Storage.JsonFile;

public static class ResourceTestBatchRunJsonFileStore
{
    public static JsonFileStore<IReadOnlyList<ResourceTestBatchRunSpecification>> Create()
    {
        return new JsonFileStore<IReadOnlyList<ResourceTestBatchRunSpecification>>(
            ResourceTestBatchRunJsonCodec.Write,
            ResourceTestBatchRunJsonCodec.Read);
    }
}
