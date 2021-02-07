using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlQueryBuilder.Common {
    public static class IEnumerableExtensions {
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
