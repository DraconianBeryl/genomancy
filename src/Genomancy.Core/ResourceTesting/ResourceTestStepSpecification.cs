using Genomancy.Core.Serialization;

namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestStepSpecification
{
    public const string ValidateSystemDefinitionKind = "validateSystemDefinition";
    public const string FreezeSystemDefinitionKind = "freezeSystemDefinition";
    public const string ExpectValidationResultKind = "expectValidationResult";
    public const string ExpectValidationDiagnosticKind = "expectValidationDiagnostic";

    public ResourceTestStepSpecification(
        string kind,
        string name = "",
        bool? expectedValid = null,
        string? diagnosticCode = null,
        bool mustBePresent = true)
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
    }

    public string Kind { get; }

    public string Name { get; }

    public bool? ExpectedValid { get; }

    public string? DiagnosticCode { get; }

    public bool MustBePresent { get; }

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
            _ => throw new GenomeSerializationException($"Unsupported resource test step kind '{Kind}'."),
        };
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
