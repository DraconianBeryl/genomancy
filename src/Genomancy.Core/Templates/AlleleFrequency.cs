using Genomancy.Core.Definitions;

namespace Genomancy.Core.Templates;

public sealed record AlleleFrequency
{
    public AlleleFrequency(ResourceId alleleId, double weight, double? numericValue = null)
    {
        if (double.IsNaN(weight) || double.IsInfinity(weight) || weight < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weight), weight, "Allele frequency weight must be finite and zero or greater.");
        }

        if (numericValue is not null && (double.IsNaN(numericValue.Value) || double.IsInfinity(numericValue.Value)))
        {
            throw new ArgumentOutOfRangeException(nameof(numericValue), numericValue, "Numeric value must be finite.");
        }

        AlleleId = alleleId;
        Weight = weight;
        NumericValue = numericValue;
    }

    public ResourceId AlleleId { get; }

    public double Weight { get; }

    public double? NumericValue { get; }
}
