using Genomancy.Core.Definitions;
using Genomancy.Core.Genome;

namespace Genomancy.Core.Templates;

public sealed record TemplatePopulationManifest
{
    public TemplatePopulationManifest(
        PopulationTemplateGroupId templateGroupId,
        PopulationTemplateGroupVersionId templateGroupVersionId,
        SystemDefinitionVersion systemDefinitionVersion,
        ulong seed,
        int requestedCount,
        string genomeVersionPrefix,
        string individualPrefix,
        IEnumerable<TemplatePopulationManifestEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        if (requestedCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(requestedCount), requestedCount, "Requested count must be zero or greater.");
        }

        TemplateGroupId = templateGroupId;
        TemplateGroupVersionId = templateGroupVersionId;
        SystemDefinitionVersion = systemDefinitionVersion;
        Seed = seed;
        RequestedCount = requestedCount;
        GenomeVersionPrefix = genomeVersionPrefix ?? string.Empty;
        IndividualPrefix = individualPrefix ?? string.Empty;
        Entries = Array.AsReadOnly(entries
            .OrderBy(entry => entry.Index)
            .ToArray());
    }

    public PopulationTemplateGroupId TemplateGroupId { get; }

    public PopulationTemplateGroupVersionId TemplateGroupVersionId { get; }

    public SystemDefinitionVersion SystemDefinitionVersion { get; }

    public ulong Seed { get; }

    public int RequestedCount { get; }

    public string GenomeVersionPrefix { get; }

    public string IndividualPrefix { get; }

    public IReadOnlyList<TemplatePopulationManifestEntry> Entries { get; }

    public bool Equals(TemplatePopulationManifest? other)
    {
        return other is not null
            && TemplateGroupId == other.TemplateGroupId
            && TemplateGroupVersionId == other.TemplateGroupVersionId
            && SystemDefinitionVersion == other.SystemDefinitionVersion
            && Seed == other.Seed
            && RequestedCount == other.RequestedCount
            && GenomeVersionPrefix == other.GenomeVersionPrefix
            && IndividualPrefix == other.IndividualPrefix
            && Entries.SequenceEqual(other.Entries);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(TemplateGroupId);
        hash.Add(TemplateGroupVersionId);
        hash.Add(SystemDefinitionVersion);
        hash.Add(Seed);
        hash.Add(RequestedCount);
        hash.Add(GenomeVersionPrefix);
        hash.Add(IndividualPrefix);

        foreach (var entry in Entries)
        {
            hash.Add(entry);
        }

        return hash.ToHashCode();
    }
}

public sealed record TemplatePopulationManifestEntry
{
    public TemplatePopulationManifestEntry(
        int index,
        ulong sampleSeed,
        GenomeVersionId genomeVersionId,
        ExternalIndividualId individualId,
        IEnumerable<PopulationTemplateGroupId> groupPath,
        PopulationTemplateId primaryTemplateId,
        PopulationTemplateVersionId primaryTemplateVersionId,
        PopulationTemplateId? secondaryTemplateId = null,
        PopulationTemplateVersionId? secondaryTemplateVersionId = null)
    {
        ArgumentNullException.ThrowIfNull(groupPath);

        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), index, "Manifest entry index must be zero or greater.");
        }

        Index = index;
        SampleSeed = sampleSeed;
        GenomeVersionId = genomeVersionId;
        IndividualId = individualId;
        GroupPath = Array.AsReadOnly(groupPath.ToArray());
        PrimaryTemplateId = primaryTemplateId;
        PrimaryTemplateVersionId = primaryTemplateVersionId;
        SecondaryTemplateId = secondaryTemplateId;
        SecondaryTemplateVersionId = secondaryTemplateVersionId;
    }

    public int Index { get; }

    public ulong SampleSeed { get; }

    public GenomeVersionId GenomeVersionId { get; }

    public ExternalIndividualId IndividualId { get; }

    public IReadOnlyList<PopulationTemplateGroupId> GroupPath { get; }

    public PopulationTemplateId PrimaryTemplateId { get; }

    public PopulationTemplateVersionId PrimaryTemplateVersionId { get; }

    public PopulationTemplateId? SecondaryTemplateId { get; }

    public PopulationTemplateVersionId? SecondaryTemplateVersionId { get; }

    public bool WasBlended => SecondaryTemplateId is not null;

    public bool Equals(TemplatePopulationManifestEntry? other)
    {
        return other is not null
            && Index == other.Index
            && SampleSeed == other.SampleSeed
            && GenomeVersionId == other.GenomeVersionId
            && IndividualId == other.IndividualId
            && GroupPath.SequenceEqual(other.GroupPath)
            && PrimaryTemplateId == other.PrimaryTemplateId
            && PrimaryTemplateVersionId == other.PrimaryTemplateVersionId
            && SecondaryTemplateId == other.SecondaryTemplateId
            && SecondaryTemplateVersionId == other.SecondaryTemplateVersionId;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Index);
        hash.Add(SampleSeed);
        hash.Add(GenomeVersionId);
        hash.Add(IndividualId);

        foreach (var group in GroupPath)
        {
            hash.Add(group);
        }

        hash.Add(PrimaryTemplateId);
        hash.Add(PrimaryTemplateVersionId);
        hash.Add(SecondaryTemplateId);
        hash.Add(SecondaryTemplateVersionId);
        return hash.ToHashCode();
    }
}
