using System.Collections.Generic;

namespace Draughts.Common.Utilities;

public static class IListExtensions {
    public static T? AtOrNull<T>(this IReadOnlyList<T> haystack, int index) where T : class {
        return haystack.Count > index ? haystack[index] : null;
    }
    public static T? AtOrNull<T>(this IList<T> haystack, int index) where T : class {
        return haystack.Count > index ? haystack[index] : null;
    }
}
