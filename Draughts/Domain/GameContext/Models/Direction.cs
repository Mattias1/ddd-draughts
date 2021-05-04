namespace Draughts.Domain.GameContext.Models {
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

        public bool IsForwardsDirection(Color color) => ForwardsYDirection(color) == DY;

        private static int ForwardsYDirection(Color color) => color == Color.Black ? 1 : -1;

        public static Direction BetweenPositions(int fromX, int fromY, int toX, int toY) {
            // Assumes from and to are on a diagonal
            return new Direction(fromX > toX ? -1 : 1, fromY > toY ? -1 : 1);
        }

        public static Direction NW => new Direction(-1, -1);
        public static Direction NE => new Direction(1, -1);
        public static Direction SE => new Direction(1, 1);
        public static Direction SW => new Direction(-1, 1);
        public static Direction[] All => new Direction[] { NW, NE, SE, SW };
    }
}
