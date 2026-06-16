namespace Genomancy.Storage.Json;

public sealed class JsonFileStoreException : Exception
{
    public JsonFileStoreException(string message)
        : base(message)
    {
    }

    public JsonFileStoreException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
