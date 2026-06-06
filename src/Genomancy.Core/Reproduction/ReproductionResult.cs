using Genomancy.Core.Genome;

namespace Genomancy.Core.Reproduction;

public sealed record ReproductionResult
{
    public ReproductionResult(
        ReproductionResultStatus status,
        GenomeVersion? offspringVersion = null,
        IEnumerable<ReproductionDiagnostic>? diagnostics = null)
    {
        Status = status;
        OffspringVersion = offspringVersion;
        Diagnostics = (diagnostics ?? [])
            .OrderBy(diagnostic => diagnostic.Path, StringComparer.Ordinal)
            .ThenBy(diagnostic => diagnostic.Code, StringComparer.Ordinal)
            .ToArray();
    }

    public ReproductionResultStatus Status { get; }

    public GenomeVersion? OffspringVersion { get; }

    public IReadOnlyList<ReproductionDiagnostic> Diagnostics { get; }
}
