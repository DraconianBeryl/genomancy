namespace Genomancy.Core.ResourceTesting;

public sealed record ValidateSystemDefinitionStep(string Name = "validate-system-definition") : IResourceTestStep
{
    public void Execute(ResourceTestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.SetValidationResult(context.SystemDefinition.Validate());
    }
}
