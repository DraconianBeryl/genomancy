using System.Text.RegularExpressions;

namespace Genomancy.Core.Definitions;

public readonly partial record struct SystemDefinitionVersion : IComparable<SystemDefinitionVersion>
{
    private readonly string _value;

    public SystemDefinitionVersion(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("System definition version must not be empty.", nameof(value));
        }

        var normalized = value.Trim();

        if (!ValidVersionPattern().IsMatch(normalized))
        {
            throw new ArgumentException(
                "System definition version may contain only letters, digits, '.', '_', '-', and '+'.",
                nameof(value));
        }

        _value = normalized;
    }

    public static SystemDefinitionVersion Parse(string value) => new(value);

    public int CompareTo(SystemDefinitionVersion other) => string.CompareOrdinal(Value, other.Value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;

    [GeneratedRegex("^[A-Za-z0-9._+-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ValidVersionPattern();
}
