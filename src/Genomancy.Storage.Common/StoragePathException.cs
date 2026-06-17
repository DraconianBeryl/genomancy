namespace Genomancy.Storage.Common;

public sealed class StoragePathException : Exception
{
    public StoragePathException(string message)
        : base(message)
    {
    }
}
