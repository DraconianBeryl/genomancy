namespace Genomancy.Core.Reproduction;

public sealed record ReproductionDiagnostic
{
    public ReproductionDiagnostic(string code, string path, string message)
    {
        Code = code;
        Path = path;
        Message = message;
    }

    public string Code { get; }

    public string Path { get; }

    public string Message { get; }
}
