using System.Text.RegularExpressions;

namespace Genomancy.Core.Mosaicism;

public readonly partial record struct InheritanceSiteId : IComparable<InheritanceSiteId>
{
    private readonly string _value;

    public InheritanceSiteId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Inheritance site id must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!ValidSiteIdPattern().IsMatch(normalized))
        {
            throw new ArgumentException(
                "Inheritance site id may contain only letters, digits, '.', '_', '-', and ':'.",
                nameof(value));
        }

        _value = normalized;
    }

    public static InheritanceSiteId Parse(string value) => new(value);

    public int CompareTo(InheritanceSiteId other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;

    [GeneratedRegex("^[A-Za-z0-9._:-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidSiteIdPattern();
}
