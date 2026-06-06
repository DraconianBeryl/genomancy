using System.Text;

namespace Genomancy.Core.Randomness;

public sealed class DeterministicRandomStream
{
    private ulong _state;

    public DeterministicRandomStream(ulong seed)
    {
        _state = seed;
    }

    public static DeterministicRandomStream FromSeedAndName(ulong rootSeed, string streamName)
    {
        if (string.IsNullOrWhiteSpace(streamName))
        {
            throw new ArgumentException("Stream name must not be empty.", nameof(streamName));
        }

        return new DeterministicRandomStream(rootSeed ^ StableHash(streamName.Trim()));
    }

    public ulong NextUInt64()
    {
        _state += 0x9E3779B97F4A7C15UL;
        var value = _state;
        value = (value ^ (value >> 30)) * 0xBF58476D1CE4E5B9UL;
        value = (value ^ (value >> 27)) * 0x94D049BB133111EBUL;
        return value ^ (value >> 31);
    }

    public int NextInt32(int exclusiveMaximum)
    {
        if (exclusiveMaximum <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(exclusiveMaximum), exclusiveMaximum, "Maximum must be greater than zero.");
        }

        return (int)(NextUInt64() % (uint)exclusiveMaximum);
    }

    public double NextUnitDouble()
    {
        return (NextUInt64() >> 11) * (1.0 / (1UL << 53));
    }

    private static ulong StableHash(string value)
    {
        const ulong offset = 14695981039346656037UL;
        const ulong prime = 1099511628211UL;
        var hash = offset;

        foreach (var valueByte in Encoding.UTF8.GetBytes(value))
        {
            hash ^= valueByte;
            hash *= prime;
        }

        return hash;
    }
}
