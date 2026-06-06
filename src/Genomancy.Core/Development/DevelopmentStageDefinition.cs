using Genomancy.Core.Definitions;

namespace Genomancy.Core.Development;

public sealed record DevelopmentStageDefinition
{
    public DevelopmentStageDefinition(ResourceId id, int order, bool requiresGestation = false)
    {
        if (order < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(order), order, "Development stage order must be zero or greater.");
        }

        Id = id;
        Order = order;
        RequiresGestation = requiresGestation;
    }

    public ResourceId Id { get; }

    public int Order { get; }

    public bool RequiresGestation { get; }
}
