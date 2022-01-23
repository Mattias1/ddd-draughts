using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlQueryBuilder.Common;

public static class IEnumerableExtensions {
    public static IReadOnlyList<T> MapReadOnly<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> func) {
        return source.Select(func).ToList().AsReadOnly();
    }
}
