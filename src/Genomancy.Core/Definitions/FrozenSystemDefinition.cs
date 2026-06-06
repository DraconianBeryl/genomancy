namespace Genomancy.Core.Definitions;

public sealed record FrozenSystemDefinition
{
    public FrozenSystemDefinition(
        SystemDefinitionVersion version,
        IEnumerable<AlleleDefinition> alleles,
        IEnumerable<GeneDefinition> genes,
        IEnumerable<GroupDefinition> groups,
        IEnumerable<BodyPlanDefinition> bodyPlans)
    {
        Version = version;
        Alleles = alleles.OrderBy(definition => definition.Id).ToReadOnlyList();
        Genes = genes.OrderBy(definition => definition.Id).ToReadOnlyList();
        Groups = groups.OrderBy(definition => definition.Id).ToReadOnlyList();
        BodyPlans = bodyPlans.OrderBy(definition => definition.Id).ToReadOnlyList();
    }

    public SystemDefinitionVersion Version { get; }

    public IReadOnlyList<AlleleDefinition> Alleles { get; }

    public IReadOnlyList<GeneDefinition> Genes { get; }

    public IReadOnlyList<GroupDefinition> Groups { get; }

    public IReadOnlyList<BodyPlanDefinition> BodyPlans { get; }
}
