using Genomancy.Core.Genome;

namespace Genomancy.Core.Mosaicism;

public static class MosaicInheritanceService
{
    public static GenomeVersion ResolveGenomeForInheritanceSite(
        MosaicGenomeState mosaicState,
        InheritanceSiteId siteId,
        IEnumerable<InheritanceSiteAssignment> siteAssignments)
    {
        ArgumentNullException.ThrowIfNull(mosaicState);
        ArgumentNullException.ThrowIfNull(siteAssignments);

        var assignment = siteAssignments.FirstOrDefault(value => value.SiteId == siteId);

        return assignment is null
            ? mosaicState.PrimaryGenomeVersion
            : mosaicState.ResolveGenomeForRegion(assignment.RegionId);
    }
}
