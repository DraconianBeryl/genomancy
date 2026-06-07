using System.Text.RegularExpressions;

namespace Genomancy.Core.Templates;

public readonly partial record struct PopulationTemplateGroupId : IComparable<PopulationTemplateGroupId>
{
    private readonly string _value;

    public PopulationTemplateGroupId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Population template group id must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!ValidTemplateGroupIdPattern().IsMatch(normalized))
        {
            throw new ArgumentException(
                "Population template group id may contain only letters, digits, '.', '_', '-', and ':'.",
                nameof(value));
        }

        _value = normalized;
    }

    public static PopulationTemplateGroupId Parse(string value) => new(value);

    public int CompareTo(PopulationTemplateGroupId other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;

    [GeneratedRegex("^[A-Za-z0-9._:-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidTemplateGroupIdPattern();
}
