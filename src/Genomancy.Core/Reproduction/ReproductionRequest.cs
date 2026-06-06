using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Reproduction;

public sealed record ReproductionRequest
{
    public ReproductionRequest(
        FrozenSystemDefinition definition,
        IEnumerable<ReproductionParentRole> parentRoles,
        ReproductionPolicy policy,
        ulong randomSeed,
        GenomeVersionId offspringVersionId,
        ExternalIndividualId offspringIndividualId)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(parentRoles);
        ArgumentNullException.ThrowIfNull(policy);

        Definition = definition;
        ParentRoles = parentRoles.ToArray();
        Policy = policy;
        RandomSeed = randomSeed;
        OffspringVersionId = offspringVersionId;
        OffspringIndividualId = offspringIndividualId;
    }

    public FrozenSystemDefinition Definition { get; }

    public IReadOnlyList<ReproductionParentRole> ParentRoles { get; }

    public ReproductionPolicy Policy { get; }

    public ulong RandomSeed { get; }

    public GenomeVersionId OffspringVersionId { get; }

    public ExternalIndividualId OffspringIndividualId { get; }
}
