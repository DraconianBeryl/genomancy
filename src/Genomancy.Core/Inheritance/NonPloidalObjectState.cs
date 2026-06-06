using Genomancy.Core.Definitions;

namespace Genomancy.Core.Inheritance;

public sealed record NonPloidalObjectState
{
    public NonPloidalObjectState(
        ResourceId id,
        NonPloidalObjectKind kind,
        double numericValue = 0,
        string textValue = "",
        bool isActive = true,
        double transmissionWeight = 1)
    {
        if (double.IsNaN(numericValue) || double.IsInfinity(numericValue))
        {
            throw new ArgumentOutOfRangeException(nameof(numericValue), numericValue, "Numeric value must be finite.");
        }

        if (double.IsNaN(transmissionWeight) || double.IsInfinity(transmissionWeight) || transmissionWeight < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(transmissionWeight), transmissionWeight, "Transmission weight must be finite and zero or greater.");
        }

        Id = id;
        Kind = kind;
        NumericValue = numericValue;
        TextValue = textValue;
        IsActive = isActive;
        TransmissionWeight = transmissionWeight;
    }

    public ResourceId Id { get; }

    public NonPloidalObjectKind Kind { get; }

    public double NumericValue { get; }

    public string TextValue { get; }

    public bool IsActive { get; }

    public double TransmissionWeight { get; }

    public NonPloidalObjectState WithNumericValue(double numericValue)
    {
        return new NonPloidalObjectState(Id, Kind, numericValue, TextValue, IsActive, TransmissionWeight);
    }

    public NonPloidalObjectState WithActive(bool isActive)
    {
        return new NonPloidalObjectState(Id, Kind, NumericValue, TextValue, isActive, TransmissionWeight);
    }
}
