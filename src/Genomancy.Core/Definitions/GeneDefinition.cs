using System.Collections.ObjectModel;

namespace Genomancy.Core.Definitions;

public sealed record GeneDefinition
{
    public GeneDefinition(
        ResourceId id,
        IEnumerable<ResourceId> alleleIds,
        IEnumerable<PolicyReference>? policyReferences = null,
        string displayName = "",
        int requiredAlleleCount = 1,
        GeneExpressionStrategy expressionStrategy = GeneExpressionStrategy.StrictDominance)
    {
        if (requiredAlleleCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requiredAlleleCount),
                requiredAlleleCount,
                "Required allele count must be greater than zero.");
        }

        Id = id;
        AlleleIds = alleleIds.ToReadOnlyList();
        PolicyReferences = (policyReferences ?? []).ToReadOnlyList();
        DisplayName = displayName;
        RequiredAlleleCount = requiredAlleleCount;
        ExpressionStrategy = expressionStrategy;
    }

    public ResourceId Id { get; }

    public IReadOnlyList<ResourceId> AlleleIds { get; }

    public IReadOnlyList<PolicyReference> PolicyReferences { get; }

    public string DisplayName { get; }

    public int RequiredAlleleCount { get; }

    public GeneExpressionStrategy ExpressionStrategy { get; }
}
