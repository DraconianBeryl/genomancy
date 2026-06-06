namespace Genomancy.Core.Randomness;

public sealed class NamedRandomStreams
{
    private readonly Dictionary<string, DeterministicRandomStream> _streams = new(StringComparer.Ordinal);

    public NamedRandomStreams(ulong rootSeed)
    {
        RootSeed = rootSeed;
    }

    public ulong RootSeed { get; }

    public DeterministicRandomStream Get(string streamName)
    {
        if (!_streams.TryGetValue(streamName, out var stream))
        {
            stream = DeterministicRandomStream.FromSeedAndName(RootSeed, streamName);
            _streams.Add(streamName, stream);
        }

        return stream;
    }
}
