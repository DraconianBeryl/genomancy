using Genomancy.Core.Definitions;

namespace Genomancy.Core.Runtime;

public sealed class GenomancySystem
{
    private GenomancySystem(
        GenomancyMode mode,
        SystemDefinitionBuilder? designDefinition,
        FrozenSystemDefinition? runtimeDefinition)
    {
        Mode = mode;
        DesignDefinition = designDefinition;
        RuntimeDefinition = runtimeDefinition;
    }

    public GenomancyMode Mode { get; }

    public SystemDefinitionBuilder? DesignDefinition { get; }

    public FrozenSystemDefinition? RuntimeDefinition { get; }

    public bool IsRuntimeFrozen => RuntimeDefinition is not null;

    public static GenomancySystem CreateDesign(SystemDefinitionVersion version)
    {
        return new GenomancySystem(GenomancyMode.Design, new SystemDefinitionBuilder(version), null);
    }

    public static GenomancySystem StartRuntime(
        SystemDefinitionBuilder sourceDefinition,
        Action<SystemDefinitionBuilder>? migrate = null)
    {
        ArgumentNullException.ThrowIfNull(sourceDefinition);

        var builder = CloneBuilder(sourceDefinition);
        migrate?.Invoke(builder);
        var frozenDefinition = builder.Freeze();

        return new GenomancySystem(GenomancyMode.Runtime, null, frozenDefinition);
    }

    private static SystemDefinitionBuilder CloneBuilder(SystemDefinitionBuilder sourceDefinition)
    {
        var clone = new SystemDefinitionBuilder(sourceDefinition.Version);

        foreach (var allele in sourceDefinition.Alleles)
        {
            clone.AddAllele(allele);
        }

        foreach (var gene in sourceDefinition.Genes)
        {
            clone.AddGene(gene);
        }

        foreach (var group in sourceDefinition.Groups)
        {
            clone.AddGroup(group);
        }

        foreach (var bodyPlan in sourceDefinition.BodyPlans)
        {
            clone.AddBodyPlan(bodyPlan);
        }

        return clone;
    }
}
