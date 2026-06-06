using Genomancy.Core.Genome;

namespace Genomancy.Core.Mutation;

public sealed record GenomeMutationRequest
{
    public GenomeMutationRequest(
        MutationSourceKind sourceKind,
        string sourceId,
        MutationApplicationMode applicationMode,
        IEnumerable<GenomeMutationOperation> operations,
        GenomeVersionId? commitVersionId = null,
        string changeSummary = "")
    {
        if (string.IsNullOrWhiteSpace(sourceId))
        {
            throw new ArgumentException("Mutation source id must not be empty.", nameof(sourceId));
        }

        SourceKind = sourceKind;
        SourceId = sourceId.Trim();
        ApplicationMode = applicationMode;
        Operations = operations.ToArray();
        CommitVersionId = commitVersionId;
        ChangeSummary = changeSummary;
    }

    public MutationSourceKind SourceKind { get; }

    public string SourceId { get; }

    public MutationApplicationMode ApplicationMode { get; }

    public IReadOnlyList<GenomeMutationOperation> Operations { get; }

    public GenomeVersionId? CommitVersionId { get; }

    public string ChangeSummary { get; }
}
