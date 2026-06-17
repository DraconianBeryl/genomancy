namespace Genomancy.Storage.Json;

public sealed record JsonFileWriteResult(
    string FullPath,
    long ByteCount,
    string Sha256Hex);
