using System.Text.RegularExpressions;

namespace Genomancy.Core.ResourceTesting;

public readonly partial record struct ResourceTestId : IComparable<ResourceTestId>
{
    private readonly string _value;

    public ResourceTestId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Resource test id must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!ValidResourceTestIdPattern().IsMatch(normalized))
        {
            throw new ArgumentException(
                "Resource test id may contain only letters, digits, '.', '_', '-', and ':'.",
                nameof(value));
        }

        _value = normalized;
    }

    public static ResourceTestId Parse(string value) => new(value);

    public int CompareTo(ResourceTestId other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;

    [GeneratedRegex("^[A-Za-z0-9._:-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidResourceTestIdPattern();
}
