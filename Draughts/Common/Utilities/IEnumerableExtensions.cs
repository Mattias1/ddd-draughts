using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Common.Utilities {
    public static class IEnumerableExtensions {
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

        public static IEnumerable<T> IntersectBy<T, TNeedle>(this IEnumerable<T> source, IEnumerable<TNeedle> needles,
                Func<T, TNeedle> comparisonFunc) {
            var set = new HashSet<TNeedle>(needles);
            return source.Where(i => set.Contains(comparisonFunc(i)));
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize) {
            var currentChunk = new List<T>(chunksize);
            foreach (var item in source) {
                currentChunk.Add(item);
                if (currentChunk.Count == chunksize) {
                    yield return currentChunk;
                    currentChunk = new List<T>(chunksize);
                }
            }
            if (currentChunk.Any()) {
                yield return currentChunk;
            }
        }
    }
}
