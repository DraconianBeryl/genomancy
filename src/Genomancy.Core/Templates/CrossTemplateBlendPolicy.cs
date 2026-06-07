namespace Genomancy.Core.Templates;

public sealed record CrossTemplateBlendPolicy
{
    public CrossTemplateBlendPolicy(double rate, double secondTemplateWeight)
    {
        if (double.IsNaN(rate) || double.IsInfinity(rate) || rate < 0 || rate > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rate), rate, "Blend rate must be between zero and one.");
        }

        if (double.IsNaN(secondTemplateWeight) || double.IsInfinity(secondTemplateWeight) || secondTemplateWeight < 0 || secondTemplateWeight > 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(secondTemplateWeight),
                secondTemplateWeight,
                "Second-template blend weight must be between zero and one.");
        }

        Rate = rate;
        SecondTemplateWeight = secondTemplateWeight;
    }

    public double Rate { get; }

    public double SecondTemplateWeight { get; }
}
