using System.Text.RegularExpressions;

namespace Genomancy.Core.Templates;

public readonly partial record struct PopulationTemplateVersionId : IComparable<PopulationTemplateVersionId>
{
    private readonly string _value;

    public PopulationTemplateVersionId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Population template version id must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!ValidTemplateVersionIdPattern().IsMatch(normalized))
        {
            throw new ArgumentException(
                "Population template version id may contain only letters, digits, '.', '_', '-', and ':'.",
                nameof(value));
        }

        _value = normalized;
    }

    public static PopulationTemplateVersionId Parse(string value) => new(value);

    public int CompareTo(PopulationTemplateVersionId other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;

    [GeneratedRegex("^[A-Za-z0-9._:-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidTemplateVersionIdPattern();
}
