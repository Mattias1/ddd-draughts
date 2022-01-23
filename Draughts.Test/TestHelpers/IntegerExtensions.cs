using Draughts.Domain.GameContext.Models;

namespace Draughts.Test.TestHelpers;

public static class IntegerExtensions {
    public static SquareId AsSquare(this int integer) => new SquareId(integer);
}
