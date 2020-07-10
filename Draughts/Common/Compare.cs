namespace Draughts.Common {
    public static class Compare {
        public static bool NullSafeEquals<T>(T? left, T? right) where T : class => left is null ? right is null : left.Equals(right);

        public static bool NullSafeNotEquals<T>(T? left, T? right) where T : class => !NullSafeEquals(left, right);
    }
}
