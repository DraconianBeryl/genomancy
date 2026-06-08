namespace Genomancy.Godot;

public sealed record GodotAdapterResult<T>
{
    public GodotAdapterResult(T? value, IEnumerable<GodotAdapterDiagnostic> diagnostics)
    {
        Value = value;
        Diagnostics = Array.AsReadOnly(diagnostics.Order().ToArray());
    }

    public T? Value { get; }

    public IReadOnlyList<GodotAdapterDiagnostic> Diagnostics { get; }

    public bool IsSuccess => Diagnostics.Count == 0;

    public static GodotAdapterResult<T> Success(T value)
    {
        return new GodotAdapterResult<T>(value, []);
    }

    public static GodotAdapterResult<T> Failure(params GodotAdapterDiagnostic[] diagnostics)
    {
        return new GodotAdapterResult<T>(default, diagnostics);
    }
}
