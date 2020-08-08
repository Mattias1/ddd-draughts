namespace Draughts.Common.Utils {
    public static class ComparisonUtils {
        public static bool NullSafeEquals<T>(T? left, T? right) where T : class => left is null ? right is null : left.Equals(right);

        public static bool NullSafeNotEquals<T>(T? left, T? right) where T : class => !NullSafeEquals(left, right);
    }
}
