using System.Text.RegularExpressions;

namespace Genomancy.Core.Genome;

public readonly partial record struct GenomeVersionId : IComparable<GenomeVersionId>
{
    private readonly string _value;

    public GenomeVersionId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Genome version id must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!ValidVersionIdPattern().IsMatch(normalized))
        {
            throw new ArgumentException(
                "Genome version id may contain only letters, digits, '.', '_', '-', and ':'.",
                nameof(value));
        }

        _value = normalized;
    }

    public static GenomeVersionId Parse(string value) => new(value);

    public int CompareTo(GenomeVersionId other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;

    [GeneratedRegex("^[A-Za-z0-9._:-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidVersionIdPattern();
}
