using Genomancy.Core.Definitions;

namespace Genomancy.Core.ResourceTesting;

public sealed record FreezeSystemDefinitionStep(string Name = "freeze-system-definition") : IResourceTestStep
{
    public void Execute(ResourceTestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            context.SetFrozenDefinition(context.SystemDefinition.Freeze());
        }
        catch (SystemDefinitionValidationException exception)
        {
            context.SetValidationResult(exception.ValidationResult);
            context.AddDiagnostic(
                ResourceTestSeverity.Error,
                "RESOURCE_TEST_FREEZE_FAILED",
                $"tests/{context.Definition.Id}/freeze",
                "System definition failed validation before freeze.");
        }
    }
}
