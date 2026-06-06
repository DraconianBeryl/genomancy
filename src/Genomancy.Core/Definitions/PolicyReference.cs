namespace Genomancy.Core.Definitions;

public sealed record PolicyReference(ResourceId Id, PolicyKind Kind);

public enum PolicyKind
{
    Inheritance,
    Expression,
    BodyPlanActivation,
    GroupDependency,
    Mutation,
    Versioning,
    Repair,
    TraceActivation,
    NonPloidalInheritance,
    Reproductive,
    GestationalCompatibility,
    TemplateGeneration,
    TemplateBlending,
    PopulationSimulation,
}
