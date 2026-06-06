using Genomancy.Core.Definitions;

namespace Genomancy.Core.Expression;

public sealed record BodyPlanAvailabilityResult
{
    public BodyPlanAvailabilityResult(
        ResourceId bodyPlanId,
        BodyPlanAvailabilityStatus status,
        IEnumerable<GroupCompletenessResult>? groupResults = null,
        IEnumerable<ExpressionDiagnostic>? diagnostics = null)
    {
        BodyPlanId = bodyPlanId;
        Status = status;
        GroupResults = (groupResults ?? [])
            .OrderBy(result => result.GroupId)
            .ToReadOnlyList();
        Diagnostics = (diagnostics ?? [])
            .Concat(GroupResults.SelectMany(result => result.Diagnostics))
            .OrderBy(diagnostic => diagnostic.Path, StringComparer.Ordinal)
            .ThenBy(diagnostic => diagnostic.Code, StringComparer.Ordinal)
            .ToReadOnlyList();
    }

    public ResourceId BodyPlanId { get; }

    public BodyPlanAvailabilityStatus Status { get; }

    public IReadOnlyList<GroupCompletenessResult> GroupResults { get; }

    public IReadOnlyList<ExpressionDiagnostic> Diagnostics { get; }
}
