using System.Security.Cryptography;

namespace Genomancy.Storage.Common;

public static class StorageFileOperations
{
    public static StoredFileWriteResult Write(
        string fullPath,
        Action<Stream> write,
        StoragePathPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(write);
        ArgumentNullException.ThrowIfNull(policy);

        var directory = Path.GetDirectoryName(fullPath)
            ?? throw new StorageFileException($"Path '{fullPath}' does not have a parent directory.");

        if (policy.CreateDirectories)
        {
            Directory.CreateDirectory(directory);
        }
        else if (!Directory.Exists(directory))
        {
            throw new StorageFileException($"Directory '{directory}' does not exist.");
        }

        if (!policy.OverwriteExisting && File.Exists(fullPath))
        {
            throw new StorageFileException($"File '{fullPath}' already exists.");
        }

        var temporaryPath = CreateTemporaryPath(fullPath, policy.TemporaryFileSuffix);

        try
        {
            using (var stream = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                write(stream);
            }

            File.Move(temporaryPath, fullPath, policy.OverwriteExisting);
            var info = new FileInfo(fullPath);
            return new StoredFileWriteResult(fullPath, info.Length, ComputeSha256Hex(fullPath));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new StorageFileException($"Failed to write {policy.StoreName} resource file '{fullPath}'.", exception);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    public static T Read<T>(string fullPath, Func<Stream, T> read, string storeName)
    {
        ArgumentNullException.ThrowIfNull(read);

        try
        {
            using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return read(stream);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new StorageFileException($"Failed to read {storeName} resource file '{fullPath}'.", exception);
        }
    }

    public static string ComputeSha256Hex(string fullPath)
    {
        using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
    }

    private static string CreateTemporaryPath(string fullPath, string temporaryFileSuffix)
    {
        var directory = Path.GetDirectoryName(fullPath)
            ?? throw new StorageFileException($"Path '{fullPath}' does not have a parent directory.");
        var fileName = Path.GetFileName(fullPath);
        return Path.Combine(directory, $"{fileName}.{Guid.NewGuid():N}{temporaryFileSuffix}");
    }
}
