using Genomancy.Core.Definitions;

namespace Genomancy.Core.Expression;

public sealed record GroupCompletenessResult
{
    public GroupCompletenessResult(
        ResourceId groupId,
        GroupCompletenessStatus status,
        IEnumerable<ExpressionDiagnostic>? diagnostics = null)
    {
        GroupId = groupId;
        Status = status;
        Diagnostics = (diagnostics ?? [])
            .OrderBy(diagnostic => diagnostic.Path, StringComparer.Ordinal)
            .ThenBy(diagnostic => diagnostic.Code, StringComparer.Ordinal)
            .ToReadOnlyList();
    }

    public ResourceId GroupId { get; }

    public GroupCompletenessStatus Status { get; }

    public IReadOnlyList<ExpressionDiagnostic> Diagnostics { get; }

    public bool IsComplete => Status == GroupCompletenessStatus.Complete;
}
