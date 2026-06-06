using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Expression;

public sealed record GeneratedComplementPolicy
{
    public GeneratedComplementPolicy(ResourceId groupId, IEnumerable<RankedAlleleSet> generatedGeneAlleles)
    {
        ArgumentNullException.ThrowIfNull(generatedGeneAlleles);

        GroupId = groupId;
        GeneratedGeneAlleles = generatedGeneAlleles
            .OrderBy(set => set.GeneId)
            .ToArray();
    }

    public ResourceId GroupId { get; }

    public IReadOnlyList<RankedAlleleSet> GeneratedGeneAlleles { get; }
}
