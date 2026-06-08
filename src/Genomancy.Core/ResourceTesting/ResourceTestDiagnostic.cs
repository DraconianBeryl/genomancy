namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestDiagnostic(
    ResourceTestSeverity Severity,
    string Code,
    string Path,
    string Message) : IComparable<ResourceTestDiagnostic>
{
    public int CompareTo(ResourceTestDiagnostic? other)
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

        return string.CompareOrdinal(Path, other.Path);
    }
}
