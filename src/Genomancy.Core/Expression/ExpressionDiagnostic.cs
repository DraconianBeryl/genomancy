using Genomancy.Core.Definitions;

namespace Genomancy.Core.Expression;

public sealed record ExpressionDiagnostic
{
    public ExpressionDiagnostic(string code, string path, ResourceId resourceId, string message)
    {
        Code = code;
        Path = path;
        ResourceId = resourceId;
        Message = message;
    }

    public string Code { get; }

    public string Path { get; }

    public ResourceId ResourceId { get; }

    public string Message { get; }
}
