using Genomancy.Core.Definitions;

namespace Genomancy.Core.Serialization;

public sealed record GenomeCompatibilityResult
{
    private GenomeCompatibilityResult(bool isCompatible, string message)
    {
        IsCompatible = isCompatible;
        Message = message;
    }

    public bool IsCompatible { get; }

    public string Message { get; }

    public static GenomeCompatibilityResult Compatible()
    {
        return new GenomeCompatibilityResult(true, string.Empty);
    }

    public static GenomeCompatibilityResult Incompatible(SystemDefinitionVersion actual, SystemDefinitionVersion expected)
    {
        return new GenomeCompatibilityResult(
            false,
            $"Genome requires system definition version '{actual}', but '{expected}' was expected.");
    }
}
