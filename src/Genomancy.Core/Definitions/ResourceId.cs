using System.Text.RegularExpressions;

namespace Genomancy.Core.Definitions;

public readonly partial record struct ResourceId : IComparable<ResourceId>
{
    private readonly string _value;

    public ResourceId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Resource id must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!ValidResourceIdPattern().IsMatch(normalized))
        {
            throw new ArgumentException(
                "Resource id may contain only letters, digits, '.', '_', '-', and ':'.",
                nameof(value));
        }

        _value = normalized;
    }

    public static ResourceId Parse(string value) => new(value);

    public int CompareTo(ResourceId other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;

    [GeneratedRegex("^[A-Za-z0-9._:-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidResourceIdPattern();
}
