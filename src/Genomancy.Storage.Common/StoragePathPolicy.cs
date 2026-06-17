namespace Genomancy.Storage.Common;

public sealed record StoragePathPolicy(
    string StoreName,
    bool CreateDirectories,
    bool OverwriteExisting,
    string TemporaryFileSuffix);
