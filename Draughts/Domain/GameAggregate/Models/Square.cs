using System.Diagnostics.CodeAnalysis;

namespace Draughts.Domain.GameAggregate.Models {
    // Right now this class is mutuable because the BoardPosition is.
    public class Square {
        private BoardPosition _board;
        public SquareId Id { get; }
        public Piece Piece { get; internal set; }

        public Square(SquareId id, Piece piece, BoardPosition board) {
            Id = id;
            Piece = piece;
            _board = board;
        }

        public bool IsEmpty => Piece.IsEmpty;
        public bool IsNotEmpty => Piece.IsNotEmpty;
        public Color? Color => Piece.Color;
        public bool IsMan => Piece.IsMan;
        public bool IsKing => Piece.IsKing;

        public (int x, int y) ToPosition() => Id.ToPosition(_board.Size);

        public bool TryGetBorder(Direction direction, [NotNullWhen(returnValue: true)] out Square? square) {
            square = GetBorder(direction);
            return square is not null;
        }

        public Square? GetBorder(Direction direction) {
            var (x, y) = ToPosition();
            if (x <= 0 && direction.DX < 0
                    || x >= _board.Size - 1 && direction.DX > 0
                    || y <= 0 && direction.DY < 0
                    || y >= _board.Size - 1 && direction.DY > 0) {
                return null;
            }
            return _board[x + direction.DX, y + direction.DY];
        }
    }
}
