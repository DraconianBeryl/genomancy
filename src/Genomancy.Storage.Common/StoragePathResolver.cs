namespace Genomancy.Storage.Common;

public sealed class StoragePathResolver
{
    private readonly string _rootDirectory;
    private readonly string _storeName;

    public StoragePathResolver(string rootDirectory, string storeName)
    {
        if (string.IsNullOrWhiteSpace(rootDirectory))
        {
            throw new ArgumentException("Root directory must not be empty.", nameof(rootDirectory));
        }

        if (string.IsNullOrWhiteSpace(storeName))
        {
            throw new ArgumentException("Store name must not be empty.", nameof(storeName));
        }

        _rootDirectory = Path.GetFullPath(rootDirectory);
        _storeName = storeName;
    }

    public string RootDirectory => _rootDirectory;

    public string ResolveRelativePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentException("Relative path must not be empty.", nameof(relativePath));
        }

        if (Path.IsPathFullyQualified(relativePath) || Path.IsPathRooted(relativePath))
        {
            throw new StoragePathException($"{_storeName} storage paths must be relative to the store root.");
        }

        var segments = relativePath.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar]);

        if (segments.Any(segment => segment is "" or "." or ".."))
        {
            throw new StoragePathException($"{_storeName} storage paths must not contain empty, current-directory, or parent-directory segments.");
        }

        var fullPath = Path.GetFullPath(Path.Combine(_rootDirectory, relativePath));
        var rootWithSeparator = _rootDirectory.EndsWith(Path.DirectorySeparatorChar)
            ? _rootDirectory
            : _rootDirectory + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(rootWithSeparator, StringComparison.Ordinal))
        {
            throw new StoragePathException($"{_storeName} storage path escapes the store root.");
        }

        return fullPath;
    }
}
