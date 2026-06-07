using System.Text.RegularExpressions;

namespace Genomancy.Core.Mosaicism;

public readonly partial record struct MosaicRegionId : IComparable<MosaicRegionId>
{
    private readonly string _value;

    public MosaicRegionId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Mosaic region id must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!ValidRegionIdPattern().IsMatch(normalized))
        {
            throw new ArgumentException(
                "Mosaic region id may contain only letters, digits, '.', '_', '-', and ':'.",
                nameof(value));
        }

        _value = normalized;
    }

    public static MosaicRegionId Parse(string value) => new(value);

    public int CompareTo(MosaicRegionId other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;

    [GeneratedRegex("^[A-Za-z0-9._:-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidRegionIdPattern();
}
