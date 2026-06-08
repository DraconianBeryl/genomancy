namespace Genomancy.Godot;

public sealed record GodotAdapterDiagnostic(
    string Code,
    string Path,
    string Message) : IComparable<GodotAdapterDiagnostic>
{
    public int CompareTo(GodotAdapterDiagnostic? other)
    {
        if (other is null)
        {
            return 1;
        }

        var codeComparison = string.CompareOrdinal(Code, other.Code);

        if (codeComparison != 0)
        {
            return codeComparison;
        }

        return string.CompareOrdinal(Path, other.Path);
    }
}
