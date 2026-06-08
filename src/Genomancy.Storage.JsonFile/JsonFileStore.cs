namespace Genomancy.Storage.JsonFile;

public sealed class JsonFileStore<T>
{
    private readonly Action<Stream, T> _write;
    private readonly Func<Stream, T> _read;

    public JsonFileStore(Action<Stream, T> write, Func<Stream, T> read)
    {
        _write = write ?? throw new ArgumentNullException(nameof(write));
        _read = read ?? throw new ArgumentNullException(nameof(read));
    }

    public void Save(string path, T value)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("JSON file path must not be empty.", nameof(path));
        }

        var directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var temporaryPath = Path.Combine(
            string.IsNullOrEmpty(directory) ? "." : directory,
            $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");

        try
        {
            using (var stream = File.Create(temporaryPath))
            {
                _write(stream, value);
            }

            File.Move(temporaryPath, path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    public T Load(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("JSON file path must not be empty.", nameof(path));
        }

        using var stream = File.OpenRead(path);
        return _read(stream);
    }
}
