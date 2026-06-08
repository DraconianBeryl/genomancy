using Genomancy.Core.Definitions;

namespace Genomancy.Core.ResourceTesting;

public sealed class ResourceTestContext
{
    private readonly List<ResourceTestDiagnostic> _diagnostics = [];

    public ResourceTestContext(ResourceTestDefinition definition, SystemDefinitionBuilder systemDefinition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(systemDefinition);

        Definition = definition;
        SystemDefinition = systemDefinition;
    }

    public ResourceTestDefinition Definition { get; }

    public SystemDefinitionBuilder SystemDefinition { get; }

    public ValidationResult? LastValidationResult { get; private set; }

    public FrozenSystemDefinition? FrozenDefinition { get; private set; }

    public IReadOnlyList<ResourceTestDiagnostic> Diagnostics => _diagnostics;

    public void SetValidationResult(ValidationResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        LastValidationResult = result;
    }

    public void SetFrozenDefinition(FrozenSystemDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        FrozenDefinition = definition;
    }

    public void AddDiagnostic(ResourceTestSeverity severity, string code, string path, string message)
    {
        _diagnostics.Add(new ResourceTestDiagnostic(severity, code, path, message));
    }
}
