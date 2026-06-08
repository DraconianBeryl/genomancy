using Genomancy.Core.Definitions;
using Genomancy.Core.Serialization;
using Genomancy.Core.Simulation;
using Genomancy.Core.Templates;

namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestStepSpecification
{
    public const string ValidateSystemDefinitionKind = "validateSystemDefinition";
    public const string FreezeSystemDefinitionKind = "freezeSystemDefinition";
    public const string ExpectValidationResultKind = "expectValidationResult";
    public const string ExpectValidationDiagnosticKind = "expectValidationDiagnostic";
    public const string AssertPopulationTemplateFrequencyKind = "assertPopulationTemplateFrequency";

    public ResourceTestStepSpecification(
        string kind,
        string name = "",
        bool? expectedValid = null,
        string? diagnosticCode = null,
        bool mustBePresent = true,
        PopulationTemplateFrequencyAssertionSpecification? populationTemplateFrequencyAssertion = null)
    {
        if (string.IsNullOrWhiteSpace(kind))
        {
            throw new ArgumentException("Resource test step kind must not be empty.", nameof(kind));
        }

        Kind = kind.Trim();
        Name = string.IsNullOrWhiteSpace(name) ? Kind : name.Trim();
        ExpectedValid = expectedValid;
        DiagnosticCode = diagnosticCode;
        MustBePresent = mustBePresent;
        PopulationTemplateFrequencyAssertion = populationTemplateFrequencyAssertion;
    }

    public string Kind { get; }

    public string Name { get; }

    public bool? ExpectedValid { get; }

    public string? DiagnosticCode { get; }

    public bool MustBePresent { get; }

    public PopulationTemplateFrequencyAssertionSpecification? PopulationTemplateFrequencyAssertion { get; }

    public IResourceTestStep ToStep()
    {
        return Kind switch
        {
            ValidateSystemDefinitionKind => new ValidateSystemDefinitionStep(Name),
            FreezeSystemDefinitionKind => new FreezeSystemDefinitionStep(Name),
            ExpectValidationResultKind => new ExpectValidationResultStep(
                ExpectedValid ?? throw new GenomeSerializationException("Expected validation result step requires 'expectedValid'."),
                Name),
            ExpectValidationDiagnosticKind => new ExpectValidationDiagnosticStep(
                Required(DiagnosticCode, "diagnosticCode"),
                MustBePresent,
                Name),
            AssertPopulationTemplateFrequencyKind => ToPopulationTemplateFrequencyStep(),
            _ => throw new GenomeSerializationException($"Unsupported resource test step kind '{Kind}'."),
        };
    }

    private AssertPopulationTemplateFrequencyStep ToPopulationTemplateFrequencyStep()
    {
        var assertion = PopulationTemplateFrequencyAssertion
            ?? throw new GenomeSerializationException("Population template frequency assertion step requires a statistical assertion payload.");

        return new AssertPopulationTemplateFrequencyStep(
            assertion.Template,
            assertion.GroupId,
            assertion.GeneId,
            assertion.AlleleId,
            assertion.SampleCount,
            assertion.Seed,
            assertion.Tolerance,
            assertion.Limits);
    }

    private static string Required(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new GenomeSerializationException($"Resource test step property '{propertyName}' is required.");
        }

        return value;
    }
}

public sealed record PopulationTemplateFrequencyAssertionSpecification
{
    public PopulationTemplateFrequencyAssertionSpecification(
        PopulationTemplateVersion template,
        ResourceId groupId,
        ResourceId geneId,
        ResourceId alleleId,
        int sampleCount,
        ulong seed,
        StatisticalTolerance tolerance,
        SimulationResourceLimits? limits = null)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(tolerance);

        Template = template;
        GroupId = groupId;
        GeneId = geneId;
        AlleleId = alleleId;
        SampleCount = sampleCount;
        Seed = seed;
        Tolerance = tolerance;
        Limits = limits;
    }

    public PopulationTemplateVersion Template { get; }

    public ResourceId GroupId { get; }

    public ResourceId GeneId { get; }

    public ResourceId AlleleId { get; }

    public int SampleCount { get; }

    public ulong Seed { get; }

    public StatisticalTolerance Tolerance { get; }

    public SimulationResourceLimits? Limits { get; }
}
