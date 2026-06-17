namespace Genomancy.Storage.Common;

public sealed record StoredResourceEntry(
    StoredResourceKind Kind,
    StoredResourceFormat Format,
    string RelativePath,
    string FullPath,
    string ResourceId,
    string ResourceVersionId,
    string SystemDefinitionVersion,
    long ByteCount,
    string Sha256Hex);
