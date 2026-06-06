namespace Genomancy.Core.Development;

public sealed record GestationContext
{
    public GestationContext(string containerId, int elapsedSteps = 0)
    {
        if (string.IsNullOrWhiteSpace(containerId))
        {
            throw new ArgumentException("Gestation container id must not be empty.", nameof(containerId));
        }

        if (elapsedSteps < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(elapsedSteps), elapsedSteps, "Elapsed gestation steps must be zero or greater.");
        }

        ContainerId = containerId.Trim();
        ElapsedSteps = elapsedSteps;
    }

    public string ContainerId { get; }

    public int ElapsedSteps { get; }
}
