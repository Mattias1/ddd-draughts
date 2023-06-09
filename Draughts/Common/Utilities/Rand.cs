using System;

namespace Draughts.Common.Utilities;

public static class Rand {
    private static readonly Random RANDOM = new Random();

    public static bool NextBool() => RANDOM.NextDouble() >= 0.5;
}
