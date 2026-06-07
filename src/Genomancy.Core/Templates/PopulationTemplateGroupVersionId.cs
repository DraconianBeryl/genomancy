using System.Text.RegularExpressions;

namespace Genomancy.Core.Templates;

public readonly partial record struct PopulationTemplateGroupVersionId : IComparable<PopulationTemplateGroupVersionId>
{
    private readonly string _value;

    public PopulationTemplateGroupVersionId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Population template group version id must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!ValidTemplateGroupVersionIdPattern().IsMatch(normalized))
        {
            throw new ArgumentException(
                "Population template group version id may contain only letters, digits, '.', '_', '-', and ':'.",
                nameof(value));
        }

        _value = normalized;
    }

    public static PopulationTemplateGroupVersionId Parse(string value) => new(value);

    public int CompareTo(PopulationTemplateGroupVersionId other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;

    [GeneratedRegex("^[A-Za-z0-9._:-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidTemplateGroupVersionIdPattern();
}
