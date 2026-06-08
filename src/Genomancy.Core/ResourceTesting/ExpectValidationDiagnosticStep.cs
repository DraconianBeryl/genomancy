namespace Genomancy.Core.ResourceTesting;

public sealed record ExpectValidationDiagnosticStep(
    string DiagnosticCode,
    bool MustBePresent = true,
    string Name = "expect-validation-diagnostic") : IResourceTestStep
{
    public void Execute(ResourceTestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.LastValidationResult is null)
        {
            context.AddDiagnostic(
                ResourceTestSeverity.Error,
                "RESOURCE_TEST_VALIDATION_MISSING",
                $"tests/{context.Definition.Id}/assertions/{Name}",
                "No validation result is available for assertion.");
            return;
        }

        var isPresent = context.LastValidationResult.Diagnostics.Any(diagnostic => diagnostic.Code == DiagnosticCode);

        if (isPresent != MustBePresent)
        {
            var expectation = MustBePresent ? "present" : "absent";
            context.AddDiagnostic(
                ResourceTestSeverity.Error,
                "RESOURCE_TEST_VALIDATION_DIAGNOSTIC_MISMATCH",
                $"tests/{context.Definition.Id}/assertions/{Name}/{DiagnosticCode}",
                $"Expected validation diagnostic '{DiagnosticCode}' to be {expectation}.");
        }
    }
}
