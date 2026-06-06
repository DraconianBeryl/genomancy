namespace Genomancy.Core.Genome;

public sealed class CurrentGenomeCopy
{
    public CurrentGenomeCopy(GenomeVersion baseVersion)
    {
        ArgumentNullException.ThrowIfNull(baseVersion);

        BaseVersion = baseVersion;
        CurrentState = new GenomeState(baseVersion.State.Groups);
    }

    public GenomeVersion BaseVersion { get; private set; }

    public GenomeState CurrentState { get; private set; }

    public bool HasUncommittedChanges => !Equals(BaseVersion.State, CurrentState);

    public void ReplaceState(GenomeState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        CurrentState = new GenomeState(state.Groups);
    }

    public void ReplaceGroup(GenomeGroupState group)
    {
        ArgumentNullException.ThrowIfNull(group);

        CurrentState = CurrentState.WithGroup(group);
    }

    public void DiscardChanges()
    {
        CurrentState = new GenomeState(BaseVersion.State.Groups);
    }

    public GenomeVersion Commit(GenomeVersionId newVersionId, string changeSummary = "")
    {
        if (newVersionId == BaseVersion.Id)
        {
            throw new ArgumentException("Committed genome version id must differ from the base version id.", nameof(newVersionId));
        }

        var committed = new GenomeVersion(
            newVersionId,
            BaseVersion.SystemDefinitionVersion,
            BaseVersion.IndividualId,
            CurrentState,
            BaseVersion.Id,
            changeSummary);

        BaseVersion = committed;
        CurrentState = new GenomeState(committed.State.Groups);

        return committed;
    }
}
