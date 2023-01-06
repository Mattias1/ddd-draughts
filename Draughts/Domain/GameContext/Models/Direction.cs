namespace Draughts.Domain.GameContext.Models;

public readonly struct Direction {
    public int DX { get; }
    public int DY { get; }

    private Direction(int dx, int dy) {
        DX = dx;
        DY = dy;
    }

    public void Deconstruct(out int dx, out int dy) {
        dx = DX;
        dy = DY;
    }

    public override string ToString() => $"({DX}, {DY})";

    public bool IsForwardsDirection(Color color) {
        int forwards = ForwardsYDirection(color);
        return DY == forwards || DY == 0 && DX == forwards;
    }

    private static int ForwardsYDirection(Color color) => color == Color.Black ? 1 : -1;

    public static Direction SQUARE_NW => new Direction(-1, -1);
    public static Direction SQUARE_NE => new Direction(1, -1);
    public static Direction SQUARE_SE => new Direction(1, 1);
    public static Direction SQUARE_SW => new Direction(-1, 1);
    public static Direction[] SQUARE_ALL => new Direction[] { SQUARE_NW, SQUARE_NE, SQUARE_SE, SQUARE_SW };

    public static Direction HEX_N => new Direction(0, -1);
    public static Direction HEX_NE => new Direction(1, -1);
    public static Direction HEX_SE => new Direction(1, 0);
    public static Direction HEX_S => new Direction(0, 1);
    public static Direction HEX_SW => new Direction(-1, 1);
    public static Direction HEX_NW => new Direction(-1, 0);
    public static Direction[] HEX_ALL => new Direction[] { HEX_N, HEX_NE, HEX_SE, HEX_S, HEX_SW, HEX_NW };
}
