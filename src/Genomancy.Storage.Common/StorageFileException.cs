namespace Genomancy.Storage.Common;

public sealed class StorageFileException : Exception
{
    public StorageFileException(string message)
        : base(message)
    {
    }

    public StorageFileException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
