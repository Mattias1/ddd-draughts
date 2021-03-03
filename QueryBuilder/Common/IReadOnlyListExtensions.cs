using System;
using System.Collections.Generic;

namespace SqlQueryBuilder.Common {
    public static class IReadOnlyListExtensions {
        public static (T, T) UnpackDuo<T>(this IReadOnlyList<T> source) {
            if (source.Count != 2) {
                throw new InvalidOperationException("Invalid amount of list items to unpack.");
            }
            return (source[0], source[1]);
        }

        public static (T, T, T) UnpackTrio<T>(this IReadOnlyList<T> source) {
            if (source.Count != 3) {
                throw new InvalidOperationException("Invalid amount of list items to unpack.");
            }
            return (source[0], source[1], source[2]);
        }

        public static (T, T, T, T) UnpackQuad<T>(this IReadOnlyList<T> source) {
            if (source.Count != 4) {
                throw new InvalidOperationException("Invalid amount of list items to unpack.");
            }
            return (source[0], source[1], source[2], source[3]);
        }
    }
}
