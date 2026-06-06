namespace Genomancy.Core.Mutation;

public sealed record GenomeMutationDiagnostic
{
    public GenomeMutationDiagnostic(string code, string path, string message)
    {
        Code = code;
        Path = path;
        Message = message;
    }

    public string Code { get; }

    public string Path { get; }

    public string Message { get; }
}
