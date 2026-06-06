using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Mutation;

public sealed record GenomeMutationOperation
{
    private GenomeMutationOperation(
        MutationOperationKind kind,
        GenomeMutationTarget target,
        ResourceId? alleleId = null,
        double? numericValue = null,
        int? copyCount = null,
        GenomeGroupState? groupState = null)
    {
        Kind = kind;
        Target = target;
        AlleleId = alleleId;
        NumericValue = numericValue;
        CopyCount = copyCount;
        GroupState = groupState;
    }

    public MutationOperationKind Kind { get; }

    public GenomeMutationTarget Target { get; }

    public ResourceId? AlleleId { get; }

    public double? NumericValue { get; }

    public int? CopyCount { get; }

    public GenomeGroupState? GroupState { get; }

    public static GenomeMutationOperation ReplaceAllele(ResourceId groupId, ResourceId geneId, int alleleRank, ResourceId alleleId)
    {
        return new GenomeMutationOperation(
            MutationOperationKind.ReplaceAllele,
            new GenomeMutationTarget(groupId, geneId, alleleRank),
            alleleId: alleleId);
    }

    public static GenomeMutationOperation UpdateNumericValue(ResourceId groupId, ResourceId geneId, int alleleRank, double numericValue)
    {
        if (double.IsNaN(numericValue) || double.IsInfinity(numericValue))
        {
            throw new ArgumentOutOfRangeException(nameof(numericValue), numericValue, "Numeric mutation value must be finite.");
        }

        return new GenomeMutationOperation(
            MutationOperationKind.UpdateNumericValue,
            new GenomeMutationTarget(groupId, geneId, alleleRank),
            numericValue: numericValue);
    }

    public static GenomeMutationOperation SetCopyCount(ResourceId groupId, ResourceId geneId, int copyCount)
    {
        if (copyCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(copyCount), copyCount, "Copy count must be greater than zero.");
        }

        return new GenomeMutationOperation(
            MutationOperationKind.SetCopyCount,
            new GenomeMutationTarget(groupId, geneId),
            copyCount: copyCount);
    }

    public static GenomeMutationOperation AddGroup(GenomeGroupState groupState)
    {
        ArgumentNullException.ThrowIfNull(groupState);

        return new GenomeMutationOperation(
            MutationOperationKind.AddGroup,
            new GenomeMutationTarget(groupState.GroupId),
            groupState: groupState);
    }

    public static GenomeMutationOperation RemoveGroup(ResourceId groupId)
    {
        return new GenomeMutationOperation(
            MutationOperationKind.RemoveGroup,
            new GenomeMutationTarget(groupId));
    }
}
