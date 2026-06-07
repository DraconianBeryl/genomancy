using Genomancy.Core.Genome;

namespace Genomancy.Core.Mosaicism;

public sealed record MosaicRegionAssignment
{
    public MosaicRegionAssignment(
        MosaicRegionId regionId,
        GenomeVersion genomeVersion,
        double coverage = 1)
    {
        ArgumentNullException.ThrowIfNull(genomeVersion);

        if (double.IsNaN(coverage) || double.IsInfinity(coverage) || coverage <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(coverage), coverage, "Coverage must be finite and greater than zero.");
        }

        RegionId = regionId;
        GenomeVersion = genomeVersion;
        Coverage = coverage;
    }

    public MosaicRegionId RegionId { get; }

    public GenomeVersion GenomeVersion { get; }

    public double Coverage { get; }
}
