using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Common.Utilities {
    public static class ComparisonUtils {
        public static bool NullSafeEquals<T>(T? left, T? right) where T : class {
            return left is null ? right is null : left.Equals(right);
        }

        public static bool NullSafeNotEquals<T>(T? left, T? right) where T : class {
            return !NullSafeEquals(left, right);
        }

        public static bool EquatableNullSafeEquals<TLeft, TRight>(TLeft? left, TRight? right)
                where TLeft : class, IEquatable<TRight>
                where TRight : class {
            return left is null ? right is null : left.Equals(right);
        }

        public static bool EquatableNullSafeNotEquals<TLeft, TRight>(TLeft? left, TRight? right)
                where TLeft : class, IEquatable<TRight>
                where TRight : class {
            return !EquatableNullSafeEquals(left, right);
        }

        public static int GetHashCode(IEnumerable<object> equalityComponents) {
            return equalityComponents.Aggregate(1, (current, obj) => {
                unchecked {
                    return current * 23 + (obj?.GetHashCode() ?? 0);
                }
            });
        }
    }
}
