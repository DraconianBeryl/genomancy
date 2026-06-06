using Genomancy.Core.Genome;

namespace Genomancy.Core.Expression;

public sealed record GeneratedComplementResult
{
    public GeneratedComplementResult(GenomeState state, bool wasGenerated, IEnumerable<ExpressionDiagnostic>? diagnostics = null)
    {
        ArgumentNullException.ThrowIfNull(state);

        State = state;
        WasGenerated = wasGenerated;
        Diagnostics = (diagnostics ?? [])
            .OrderBy(diagnostic => diagnostic.Path, StringComparer.Ordinal)
            .ThenBy(diagnostic => diagnostic.Code, StringComparer.Ordinal)
            .ToArray();
    }

    public GenomeState State { get; }

    public bool WasGenerated { get; }

    public IReadOnlyList<ExpressionDiagnostic> Diagnostics { get; }
}
