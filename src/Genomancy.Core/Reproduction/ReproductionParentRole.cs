using Genomancy.Core.Genome;

namespace Genomancy.Core.Reproduction;

public sealed record ReproductionParentRole
{
    public ReproductionParentRole(string name, GenomeVersion genomeVersion)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Parent role name must not be empty.", nameof(name));
        }

        ArgumentNullException.ThrowIfNull(genomeVersion);

        Name = name.Trim();
        GenomeVersion = genomeVersion;
    }

    public string Name { get; }

    public GenomeVersion GenomeVersion { get; }
}
