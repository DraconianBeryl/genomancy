using Genomancy.Core.Genome;

namespace Genomancy.Core.Mosaicism;

public sealed record MosaicGenomeState
{
    public MosaicGenomeState(
        GenomeVersion primaryGenomeVersion,
        IEnumerable<MosaicRegionAssignment>? regions = null,
        IEnumerable<ChimericMaterialState>? chimericMaterials = null)
    {
        ArgumentNullException.ThrowIfNull(primaryGenomeVersion);

        PrimaryGenomeVersion = primaryGenomeVersion;
        Regions = (regions ?? [])
            .OrderBy(region => region.RegionId)
            .ToArray();
        ChimericMaterials = (chimericMaterials ?? [])
            .OrderBy(material => material.Id)
            .ToArray();
    }

    public GenomeVersion PrimaryGenomeVersion { get; }

    public IReadOnlyList<MosaicRegionAssignment> Regions { get; }

    public IReadOnlyList<ChimericMaterialState> ChimericMaterials { get; }

    public GenomeVersion ResolveGenomeForRegion(MosaicRegionId regionId)
    {
        return Regions.FirstOrDefault(region => region.RegionId == regionId)?.GenomeVersion
            ?? PrimaryGenomeVersion;
    }
}
