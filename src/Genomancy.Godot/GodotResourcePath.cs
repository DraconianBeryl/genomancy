namespace Genomancy.Godot;

public readonly record struct GodotResourcePath : IComparable<GodotResourcePath>
{
    private readonly string _value;

    public GodotResourcePath(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Godot resource path must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!normalized.StartsWith("res://", StringComparison.Ordinal)
            && !normalized.StartsWith("user://", StringComparison.Ordinal))
        {
            throw new ArgumentException("Godot resource path must start with 'res://' or 'user://'.", nameof(value));
        }

        _value = normalized;
    }

    public static GodotResourcePath Parse(string value) => new(value);

    public int CompareTo(GodotResourcePath other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;
}
