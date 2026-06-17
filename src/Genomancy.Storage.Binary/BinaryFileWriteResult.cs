using Genomancy.Storage.Common;

namespace Genomancy.Storage.Binary;

public sealed record BinaryFileWriteResult(
    string FullPath,
    long ByteCount,
    string Sha256Hex,
    StoredResourceEntry ManifestEntry);
