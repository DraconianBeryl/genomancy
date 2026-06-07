namespace Genomancy.Core.Templates;

public sealed record WeightedPopulationTemplateGroup
{
    public WeightedPopulationTemplateGroup(PopulationTemplateGroupVersion group, double weight)
    {
        ArgumentNullException.ThrowIfNull(group);

        if (double.IsNaN(weight) || double.IsInfinity(weight) || weight < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weight), weight, "Template-group selection weight must be finite and zero or greater.");
        }

        Group = group;
        Weight = weight;
    }

    public PopulationTemplateGroupVersion Group { get; }

    public double Weight { get; }
}
