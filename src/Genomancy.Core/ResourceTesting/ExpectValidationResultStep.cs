namespace Genomancy.Core.ResourceTesting;

public sealed record ExpectValidationResultStep(bool ExpectedValid, string Name = "expect-validation-result") : IResourceTestStep
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

        if (context.LastValidationResult.IsValid != ExpectedValid)
        {
            context.AddDiagnostic(
                ResourceTestSeverity.Error,
                "RESOURCE_TEST_VALIDATION_RESULT_MISMATCH",
                $"tests/{context.Definition.Id}/assertions/{Name}",
                $"Expected validation IsValid={ExpectedValid}, got IsValid={context.LastValidationResult.IsValid}.");
        }
    }
}
