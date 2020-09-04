using System;

namespace Draughts.Common.Utilities {
    public static class Rand {
        private static Random Random = new Random();

        public static bool NextBool() => Random.NextDouble() >= 0.5;
    }
}
