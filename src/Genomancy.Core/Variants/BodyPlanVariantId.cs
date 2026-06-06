using System.Text.RegularExpressions;

namespace Genomancy.Core.Variants;

public readonly partial record struct BodyPlanVariantId : IComparable<BodyPlanVariantId>
{
    private readonly string _value;

    public BodyPlanVariantId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Body-plan variant id must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!ValidVariantIdPattern().IsMatch(normalized))
        {
            throw new ArgumentException(
                "Body-plan variant id may contain only letters, digits, '.', '_', '-', and ':'.",
                nameof(value));
        }

        _value = normalized;
    }

    public static BodyPlanVariantId Parse(string value) => new(value);

    public int CompareTo(BodyPlanVariantId other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;

    [GeneratedRegex("^[A-Za-z0-9._:-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidVariantIdPattern();
}
