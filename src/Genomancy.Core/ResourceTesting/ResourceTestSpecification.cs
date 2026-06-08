using Genomancy.Core.Definitions;

namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestSpecification
{
    public ResourceTestSpecification(
        ResourceTestId id,
        SystemDefinitionVersion systemDefinitionVersion,
        ResourceTestFixtureSpecification fixture,
        IEnumerable<ResourceTestStepSpecification> steps,
        string displayName = "",
        IEnumerable<string>? tags = null)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        ArgumentNullException.ThrowIfNull(steps);

        Id = id;
        SystemDefinitionVersion = systemDefinitionVersion;
        Fixture = fixture;
        Steps = Array.AsReadOnly(steps.ToArray());
        DisplayName = displayName;
        Tags = Array.AsReadOnly((tags ?? [])
            .Select(tag => tag.Trim())
            .Where(tag => tag.Length > 0)
            .Order(StringComparer.Ordinal)
            .ToArray());

        if (Steps.Count == 0)
        {
            throw new ArgumentException("Resource test specification must contain at least one step.", nameof(steps));
        }
    }

    public ResourceTestId Id { get; }

    public SystemDefinitionVersion SystemDefinitionVersion { get; }

    public ResourceTestFixtureSpecification Fixture { get; }

    public IReadOnlyList<ResourceTestStepSpecification> Steps { get; }

    public string DisplayName { get; }

    public IReadOnlyList<string> Tags { get; }

    public ResourceTestDefinition ToDefinition()
    {
        return new ResourceTestDefinition(
            Id,
            () => Fixture.CreateBuilder(SystemDefinitionVersion),
            Steps.Select(step => step.ToStep()),
            DisplayName,
            Tags);
    }
}

public sealed record ResourceTestFixtureSpecification
{
    public ResourceTestFixtureSpecification(
        IEnumerable<AlleleResourceSpecification>? alleles = null,
        IEnumerable<GeneResourceSpecification>? genes = null,
        IEnumerable<GroupResourceSpecification>? groups = null,
        IEnumerable<BodyPlanResourceSpecification>? bodyPlans = null)
    {
        Alleles = Array.AsReadOnly((alleles ?? []).OrderBy(allele => allele.Id).ToArray());
        Genes = Array.AsReadOnly((genes ?? []).OrderBy(gene => gene.Id).ToArray());
        Groups = Array.AsReadOnly((groups ?? []).OrderBy(group => group.Id).ToArray());
        BodyPlans = Array.AsReadOnly((bodyPlans ?? []).OrderBy(bodyPlan => bodyPlan.Id).ToArray());
    }

    public IReadOnlyList<AlleleResourceSpecification> Alleles { get; }

    public IReadOnlyList<GeneResourceSpecification> Genes { get; }

    public IReadOnlyList<GroupResourceSpecification> Groups { get; }

    public IReadOnlyList<BodyPlanResourceSpecification> BodyPlans { get; }

    public SystemDefinitionBuilder CreateBuilder(SystemDefinitionVersion version)
    {
        var builder = new SystemDefinitionBuilder(version);

        foreach (var allele in Alleles)
        {
            builder.AddAllele(new AlleleDefinition(allele.Id, allele.DisplayName));
        }

        foreach (var gene in Genes)
        {
            builder.AddGene(new GeneDefinition(
                gene.Id,
                gene.AlleleIds,
                displayName: gene.DisplayName,
                requiredAlleleCount: gene.RequiredAlleleCount,
                expressionStrategy: gene.ExpressionStrategy));
        }

        foreach (var group in Groups)
        {
            builder.AddGroup(new GroupDefinition(
                group.Id,
                group.GeneIds,
                group.SubgroupIds,
                group.DependencyGroupIds,
                displayName: group.DisplayName));
        }

        foreach (var bodyPlan in BodyPlans)
        {
            builder.AddBodyPlan(new BodyPlanDefinition(
                bodyPlan.Id,
                bodyPlan.RequiredGroupIds,
                bodyPlan.OptionalGroupIds,
                bodyPlan.SharedGroupIds,
                displayName: bodyPlan.DisplayName));
        }

        return builder;
    }
}

public sealed record AlleleResourceSpecification(ResourceId Id, string DisplayName = "");

public sealed record GeneResourceSpecification
{
    public GeneResourceSpecification(
        ResourceId id,
        IEnumerable<ResourceId> alleleIds,
        string displayName = "",
        int requiredAlleleCount = 1,
        GeneExpressionStrategy expressionStrategy = GeneExpressionStrategy.StrictDominance)
    {
        Id = id;
        AlleleIds = Array.AsReadOnly(alleleIds.Order().ToArray());
        DisplayName = displayName;
        RequiredAlleleCount = requiredAlleleCount;
        ExpressionStrategy = expressionStrategy;
    }

    public ResourceId Id { get; }

    public IReadOnlyList<ResourceId> AlleleIds { get; }

    public string DisplayName { get; }

    public int RequiredAlleleCount { get; }

    public GeneExpressionStrategy ExpressionStrategy { get; }
}

public sealed record GroupResourceSpecification
{
    public GroupResourceSpecification(
        ResourceId id,
        IEnumerable<ResourceId>? geneIds = null,
        IEnumerable<ResourceId>? subgroupIds = null,
        IEnumerable<ResourceId>? dependencyGroupIds = null,
        string displayName = "")
    {
        Id = id;
        GeneIds = Array.AsReadOnly((geneIds ?? []).Order().ToArray());
        SubgroupIds = Array.AsReadOnly((subgroupIds ?? []).Order().ToArray());
        DependencyGroupIds = Array.AsReadOnly((dependencyGroupIds ?? []).Order().ToArray());
        DisplayName = displayName;
    }

    public ResourceId Id { get; }

    public IReadOnlyList<ResourceId> GeneIds { get; }

    public IReadOnlyList<ResourceId> SubgroupIds { get; }

    public IReadOnlyList<ResourceId> DependencyGroupIds { get; }

    public string DisplayName { get; }
}

public sealed record BodyPlanResourceSpecification
{
    public BodyPlanResourceSpecification(
        ResourceId id,
        IEnumerable<ResourceId> requiredGroupIds,
        IEnumerable<ResourceId>? optionalGroupIds = null,
        IEnumerable<ResourceId>? sharedGroupIds = null,
        string displayName = "")
    {
        Id = id;
        RequiredGroupIds = Array.AsReadOnly(requiredGroupIds.Order().ToArray());
        OptionalGroupIds = Array.AsReadOnly((optionalGroupIds ?? []).Order().ToArray());
        SharedGroupIds = Array.AsReadOnly((sharedGroupIds ?? []).Order().ToArray());
        DisplayName = displayName;
    }

    public ResourceId Id { get; }

    public IReadOnlyList<ResourceId> RequiredGroupIds { get; }

    public IReadOnlyList<ResourceId> OptionalGroupIds { get; }

    public IReadOnlyList<ResourceId> SharedGroupIds { get; }

    public string DisplayName { get; }
}
