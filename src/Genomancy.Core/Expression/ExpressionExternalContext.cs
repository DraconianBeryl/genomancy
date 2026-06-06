namespace Genomancy.Core.Expression;

public sealed record ExpressionExternalContext
{
    public ExpressionExternalContext(IEnumerable<KeyValuePair<string, string>>? facts = null)
    {
        Facts = (facts ?? [])
            .OrderBy(fact => fact.Key, StringComparer.Ordinal)
            .ThenBy(fact => fact.Value, StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyList<KeyValuePair<string, string>> Facts { get; }
}
