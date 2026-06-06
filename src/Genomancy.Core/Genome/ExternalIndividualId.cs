namespace Genomancy.Core.Genome;

public readonly record struct ExternalIndividualId
{
    private readonly string _value;

    public ExternalIndividualId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("External individual id must not be empty.", nameof(value));
        }

        _value = value.Trim();
    }

    public static ExternalIndividualId Parse(string value) => new(value);

    public override string ToString() => Value;

    public string Value => _value ?? string.Empty;
}
