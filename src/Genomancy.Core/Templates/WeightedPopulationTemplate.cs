namespace Genomancy.Core.Templates;

public sealed record WeightedPopulationTemplate
{
    public WeightedPopulationTemplate(PopulationTemplateVersion template, double weight)
    {
        ArgumentNullException.ThrowIfNull(template);

        if (double.IsNaN(weight) || double.IsInfinity(weight) || weight < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weight), weight, "Template selection weight must be finite and zero or greater.");
        }

        Template = template;
        Weight = weight;
    }

    public PopulationTemplateVersion Template { get; }

    public double Weight { get; }
}
