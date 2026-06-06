namespace Genomancy.Core.Definitions;

public sealed record ValidationDiagnostic(
    ValidationSeverity Severity,
    string Code,
    string Path,
    ResourceId? ResourceId,
    string Message) : IComparable<ValidationDiagnostic>
{
    public int CompareTo(ValidationDiagnostic? other)
    {
        if (other is null)
        {
            return 1;
        }

        var severityComparison = Severity.CompareTo(other.Severity);

        if (severityComparison != 0)
        {
            return severityComparison;
        }

        var codeComparison = string.CompareOrdinal(Code, other.Code);

        if (codeComparison != 0)
        {
            return codeComparison;
        }

        var pathComparison = string.CompareOrdinal(Path, other.Path);

        if (pathComparison != 0)
        {
            return pathComparison;
        }

        return string.CompareOrdinal(ResourceId?.Value ?? string.Empty, other.ResourceId?.Value ?? string.Empty);
    }
}

public enum ValidationSeverity
{
    Error = 0,
    Warning = 1,
}
