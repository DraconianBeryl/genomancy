using Genomancy.Core.Genome;

namespace Genomancy.Core.Mutation;

public sealed record GenomeMutationResult
{
    public GenomeMutationResult(
        GenomeMutationResultStatus status,
        GenomeVersion? committedVersion = null,
        IEnumerable<GenomeMutationDiagnostic>? diagnostics = null)
    {
        Status = status;
        CommittedVersion = committedVersion;
        Diagnostics = (diagnostics ?? [])
            .OrderBy(diagnostic => diagnostic.Path, StringComparer.Ordinal)
            .ThenBy(diagnostic => diagnostic.Code, StringComparer.Ordinal)
            .ToArray();
    }

    public GenomeMutationResultStatus Status { get; }

    public GenomeVersion? CommittedVersion { get; }

    public IReadOnlyList<GenomeMutationDiagnostic> Diagnostics { get; }
}
