namespace Genomancy.Core.Definitions;

public sealed class DefinitionMutationException : InvalidOperationException
{
    public DefinitionMutationException(string message)
        : base(message)
    {
    }
}
