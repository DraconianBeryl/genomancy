using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Mosaicism;

public sealed record ChimericMaterialState
{
    public ChimericMaterialState(
        ResourceId id,
        GenomeVersion genomeVersion,
        IEnumerable<MosaicRegionId>? expressedRegionIds = null,
        bool isIntegratedBodyPlanVariant = false)
    {
        ArgumentNullException.ThrowIfNull(genomeVersion);

        Id = id;
        GenomeVersion = genomeVersion;
        ExpressedRegionIds = (expressedRegionIds ?? []).Order().ToArray();
        IsIntegratedBodyPlanVariant = isIntegratedBodyPlanVariant;
    }

    public ResourceId Id { get; }

    public GenomeVersion GenomeVersion { get; }

    public IReadOnlyList<MosaicRegionId> ExpressedRegionIds { get; }

    public bool IsIntegratedBodyPlanVariant { get; }
}
