using System.Text.RegularExpressions;

namespace Genomancy.Core.Expression;

public readonly partial record struct DevelopmentalPhaseId : IComparable<DevelopmentalPhaseId>
{
    private readonly string _value;

    public DevelopmentalPhaseId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Developmental phase id must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!ValidDevelopmentalPhaseIdPattern().IsMatch(normalized))
        {
            throw new ArgumentException(
                "Developmental phase id may contain only letters, digits, '.', '_', '-', and ':'.",
                nameof(value));
        }

        _value = normalized;
    }

    public static DevelopmentalPhaseId Parse(string value) => new(value);

    public int CompareTo(DevelopmentalPhaseId other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;

    [GeneratedRegex("^[A-Za-z0-9._:-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidDevelopmentalPhaseIdPattern();
}
