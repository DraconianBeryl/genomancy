namespace Genomancy.Core.Definitions;

public sealed record ValidationResult
{
    public ValidationResult(IEnumerable<ValidationDiagnostic> diagnostics)
    {
        Diagnostics = diagnostics.Order().ToReadOnlyList();
    }

    public IReadOnlyList<ValidationDiagnostic> Diagnostics { get; }

    public bool IsValid => Diagnostics.All(diagnostic => diagnostic.Severity != ValidationSeverity.Error);
}
