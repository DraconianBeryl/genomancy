namespace Genomancy.Core.Definitions;

public sealed class SystemDefinitionValidationException : InvalidOperationException
{
    public SystemDefinitionValidationException(ValidationResult validationResult)
        : base("System definition validation failed.")
    {
        ValidationResult = validationResult;
    }

    public ValidationResult ValidationResult { get; }
}
