using System.Collections.ObjectModel;

namespace Genomancy.Core.Definitions;

internal static class CollectionExtensions
{
    public static ReadOnlyCollection<T> ToReadOnlyList<T>(this IEnumerable<T> values)
    {
        return Array.AsReadOnly(values.ToArray());
    }
}
