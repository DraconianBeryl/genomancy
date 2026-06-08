namespace Genomancy.Core.ResourceTesting;

public sealed record ResourceTestRunOptions
{
    public ResourceTestRunOptions(IEnumerable<string>? includeTags = null, IEnumerable<string>? excludeTags = null)
    {
        IncludeTags = NormalizeTags(includeTags);
        ExcludeTags = NormalizeTags(excludeTags);
    }

    public IReadOnlySet<string> IncludeTags { get; }

    public IReadOnlySet<string> ExcludeTags { get; }

    public bool ShouldRun(ResourceTestDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (ExcludeTags.Count > 0 && definition.Tags.Any(ExcludeTags.Contains))
        {
            return false;
        }

        return IncludeTags.Count == 0 || definition.Tags.Any(IncludeTags.Contains);
    }

    private static IReadOnlySet<string> NormalizeTags(IEnumerable<string>? tags)
    {
        return tags is null
            ? new HashSet<string>(StringComparer.Ordinal)
            : tags
                .Select(tag => tag.Trim())
                .Where(tag => tag.Length > 0)
                .ToHashSet(StringComparer.Ordinal);
    }
}
