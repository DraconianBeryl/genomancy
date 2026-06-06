namespace Genomancy.Core.Inheritance;

public sealed record NonPloidalInheritanceResult
{
    public NonPloidalInheritanceResult(HeritableObjectState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        State = state;
    }

    public HeritableObjectState State { get; }
}
