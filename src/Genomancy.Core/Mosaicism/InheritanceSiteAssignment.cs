namespace Genomancy.Core.Mosaicism;

public sealed record InheritanceSiteAssignment
{
    public InheritanceSiteAssignment(InheritanceSiteId siteId, MosaicRegionId regionId)
    {
        SiteId = siteId;
        RegionId = regionId;
    }

    public InheritanceSiteId SiteId { get; }

    public MosaicRegionId RegionId { get; }
}
