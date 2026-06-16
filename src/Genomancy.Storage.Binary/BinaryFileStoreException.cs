namespace Genomancy.Storage.Binary;

public sealed class BinaryFileStoreException : Exception
{
    public BinaryFileStoreException(string message)
        : base(message)
    {
    }

    public BinaryFileStoreException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
