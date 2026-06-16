namespace Genomancy.Storage.Binary;

public sealed record BinaryFileStoreOptions
{
    public BinaryFileStoreOptions(
        bool createDirectories = true,
        bool overwriteExisting = true,
        string temporaryFileSuffix = ".tmp")
    {
        if (string.IsNullOrWhiteSpace(temporaryFileSuffix))
        {
            throw new ArgumentException("Temporary file suffix must not be empty.", nameof(temporaryFileSuffix));
        }

        CreateDirectories = createDirectories;
        OverwriteExisting = overwriteExisting;
        TemporaryFileSuffix = temporaryFileSuffix;
    }

    public bool CreateDirectories { get; }

    public bool OverwriteExisting { get; }

    public string TemporaryFileSuffix { get; }
}
