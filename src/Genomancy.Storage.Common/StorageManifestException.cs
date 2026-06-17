namespace Genomancy.Storage.Common;

public sealed class StorageManifestException : Exception
{
    public StorageManifestException(string message)
        : base(message)
    {
    }

    public StorageManifestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
