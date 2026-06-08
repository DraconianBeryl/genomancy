namespace Genomancy.Godot;

public sealed record GodotResourceDocument
{
    public GodotResourceDocument(
        GodotResourcePath path,
        string kind,
        string payloadJson,
        string systemDefinitionVersion = "",
        IEnumerable<string>? tags = null)
    {
        if (string.IsNullOrWhiteSpace(kind))
        {
            throw new ArgumentException("Godot resource kind must not be empty.", nameof(kind));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(payloadJson);

        Path = path;
        Kind = kind.Trim();
        PayloadJson = payloadJson;
        SystemDefinitionVersion = systemDefinitionVersion.Trim();
        Tags = Array.AsReadOnly((tags ?? [])
            .Select(tag => tag.Trim())
            .Where(tag => tag.Length > 0)
            .Order(StringComparer.Ordinal)
            .ToArray());
    }

    public GodotResourcePath Path { get; }

    public string Kind { get; }

    public string PayloadJson { get; }

    public string SystemDefinitionVersion { get; }

    public IReadOnlyList<string> Tags { get; }
}
