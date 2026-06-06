using Genomancy.Core.Definitions;

namespace Genomancy.Core.Reproduction;

public sealed record TransmissionWeight
{
    public TransmissionWeight(string roleName, ResourceId groupId, ResourceId geneId, ResourceId alleleId, double weight)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            throw new ArgumentException("Role name must not be empty.", nameof(roleName));
        }

        if (double.IsNaN(weight) || double.IsInfinity(weight) || weight < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weight), weight, "Transmission weight must be finite and zero or greater.");
        }

        RoleName = roleName.Trim();
        GroupId = groupId;
        GeneId = geneId;
        AlleleId = alleleId;
        Weight = weight;
    }

    public string RoleName { get; }

    public ResourceId GroupId { get; }

    public ResourceId GeneId { get; }

    public ResourceId AlleleId { get; }

    public double Weight { get; }
}
