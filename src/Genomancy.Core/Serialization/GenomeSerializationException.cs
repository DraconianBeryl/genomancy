namespace Genomancy.Core.Serialization;

public sealed class GenomeSerializationException : Exception
{
    public GenomeSerializationException(string message)
        : base(message)
    {
    }

    public GenomeSerializationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
