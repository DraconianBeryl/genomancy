namespace Genomancy.Storage.Common;

public sealed record StoredFileWriteResult(
    string FullPath,
    long ByteCount,
    string Sha256Hex);
