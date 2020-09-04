using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Common.Utilities {
    public static class LinqExtensions {
        public static bool ContainsAll<T>(this IEnumerable<T> haystack, IEnumerable<T> needles) {
            var set = new HashSet<T>(haystack);
            return needles.All(n => set.Contains(n));
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> func) {
            foreach (T item in source) {
                func(item);
            }
            return source;
        }
    }
}
