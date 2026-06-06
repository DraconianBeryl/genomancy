using Genomancy.Core.Definitions;

namespace Genomancy.Core.Genome;

public sealed record GenomeVersion
{
    public GenomeVersion(
        GenomeVersionId id,
        SystemDefinitionVersion systemDefinitionVersion,
        ExternalIndividualId individualId,
        GenomeState state,
        GenomeVersionId? parentVersionId = null,
        string changeSummary = "")
    {
        ArgumentNullException.ThrowIfNull(state);

        Id = id;
        SystemDefinitionVersion = systemDefinitionVersion;
        IndividualId = individualId;
        ParentVersionId = parentVersionId;
        State = new GenomeState(state.Groups);
        ChangeSummary = changeSummary;
    }

    public GenomeVersionId Id { get; }

    public SystemDefinitionVersion SystemDefinitionVersion { get; }

    public ExternalIndividualId IndividualId { get; }

    public GenomeVersionId? ParentVersionId { get; }

    public GenomeState State { get; }

    public string ChangeSummary { get; }
}
