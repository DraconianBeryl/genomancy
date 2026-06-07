using Genomancy.Core.Expression;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Mosaicism;

public sealed record MosaicExpressionResult
{
    public MosaicExpressionResult(MosaicRegionId regionId, GenomeVersion genomeVersion, GeneExpressionResult expression)
    {
        ArgumentNullException.ThrowIfNull(genomeVersion);
        ArgumentNullException.ThrowIfNull(expression);

        RegionId = regionId;
        GenomeVersion = genomeVersion;
        Expression = expression;
    }

    public MosaicRegionId RegionId { get; }

    public GenomeVersion GenomeVersion { get; }

    public GeneExpressionResult Expression { get; }
}
