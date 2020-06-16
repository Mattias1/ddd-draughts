using System.Collections.Generic;
using System.Linq;

namespace Draughts.Common {
    public static class LinqExtensions {
        public static bool ContainsAll<T>(this IEnumerable<T> haystack, IEnumerable<T> needles) {
            var set = new HashSet<T>(haystack);
            return needles.All(n => set.Contains(n));
        }
    }
}
