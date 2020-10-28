using Draughts.Common;
using Draughts.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Draughts.Domain.GameAggregate.Models {
    // This class is like a mutuable value object. It could be immutable, but that'd be not very performant. Maybe. Hmmm. :/
    public class BoardPosition : IEquatable<BoardPosition> {
        private readonly Piece[,] _squares;

        /// <summary>
        /// The square at (x, y).
        /// The top left square is (0, 0).
        /// </summary>
        public Piece this[int x, int y] => _squares[x, y];
        public Piece this[SquareNumber n] {
            get {
                var (x, y) = n.ToPosition(Size);
                return this[x, y];
            }
        }

        public int Size => _squares.GetLength(0);

        private BoardPosition(Piece[,] squares) => _squares = squares;

        public void Move(SquareNumber from, SquareNumber to) {
            if (!IsMove(from, to)) {
                throw new ManualValidationException("Invalid move.");
            }

            var (fromX, fromY, toX, toY) = ExtractPositions(from, to);
            _squares[toX, toY] = _squares[fromX, fromY];
            _squares[fromX, fromY] = Piece.Empty;
        }

        public void Capture(SquareNumber from, SquareNumber to) {
            if (!IsCapture(from, to)) {
                throw new ManualValidationException("Invalid capture.");
            }

            var (fromX, fromY, toX, toY) = ExtractPositions(from, to);
            var (victimX, victimY) = OnlyNonEmptySpotBetween(from, to)!.ToPosition(Size);
            _squares[toX, toY] = _squares[fromX, fromY];
            _squares[fromX, fromY] = Piece.Empty;
            _squares[victimX, victimY] = Piece.Empty;
        }

        // TODO: (non-)flying kings, etc.
        public bool IsMove(SquareNumber from, SquareNumber to) {
            return this[from].IsMan ? IsManMove(from, to) : IsKingMove(from, to);
        }

        public bool IsManMove(SquareNumber from, SquareNumber to) {
            var (_, fromY, _, toY) = ExtractPositions(from, to);
            return this[from].IsNotEmpty && this[to].IsEmpty
                && DiagonalDistance(from, to) == 1
                && fromY + ForwardsYDirection(from) == toY;
        }

        public bool IsKingMove(SquareNumber from, SquareNumber to) {
            return this[from].IsNotEmpty
                && IsDiagonalMove(from, to, out _)
                && IsEmptyDiagonal(from, to);
        }

        public bool IsCapture(SquareNumber from, SquareNumber to) {
            int distance = DiagonalDistance(from, to);
            if (this[from].IsEmpty || this[to].IsNotEmpty || distance < 2 || distance > 2 && this[from].IsMan) {
                return false;
            }
            var pos = OnlyNonEmptySpotBetween(from, to);
            return pos != null && this[pos].Color != this[from].Color;
        }

        private int DiagonalDistance(SquareNumber from, SquareNumber to) {
            if (!IsDiagonalMove(from, to, out int distance)) {
                throw new InvalidOperationException("Every move should be along a diagonal.");
            }
            return distance;
        }

        private bool IsDiagonalMove(SquareNumber from, SquareNumber to, out int distance) {
            var (fromX, fromY, toX, toY) = ExtractPositions(from, to);
            distance = Math.Abs(fromX - toX);
            return Math.Abs(fromY - toY) == distance;
        }

        private SquareNumber? OnlyNonEmptySpotBetween(SquareNumber from, SquareNumber to) {
            // Assumes from and to are on a diagonal
            SquareNumber? result = null;
            var (x, y, toX, toY) = ExtractPositions(from, to);
            var (dirX, dirY) = Direction(from, to);
            while (x != toX) {
                (x, y) = (x + dirX, y + dirY);
                if (_squares[x, y].IsNotEmpty) {
                    if (result is null) {
                        result = SquareNumber.FromPosition(x, y, Size);
                    }
                    else {
                        return null;
                    }
                }
            }
            if (_squares[toX, toY].IsNotEmpty) {
                return null;
            }
            return result;
        }

        private bool IsEmptyDiagonal(SquareNumber from, SquareNumber to) {
            // Assumes from and to are on a diagonal
            var (x, y, toX, _) = ExtractPositions(from, to);
            var (dirX, dirY) = Direction(from, to);
            while (x != toX) {
                (x, y) = (x + dirX, y + dirY);
                if (_squares[x, y].IsNotEmpty) {
                    return false;
                }
            }
            return true;
        }

        private (int, int) Direction(SquareNumber from, SquareNumber to) {
            // Assumes from and to are on a diagonal
            var (fromX, fromY, toX, toY) = ExtractPositions(from, to);
            return (fromX > toX ? -1 : 1, fromY > toY ? -1 : 1);
        }

        private int ForwardsYDirection(SquareNumber from) {
            var color = this[from].Color;
            if (color is null) {
                throw new ManualValidationException("Invalid move.");
            }
            return color == Color.Black ? 1 : -1;
        }

        private (int, int, int, int) ExtractPositions(SquareNumber from, SquareNumber to) {
            var (fromX, fromY) = from.ToPosition(Size);
            var (toX, toY) = to.ToPosition(Size);
            return (fromX, fromY, toX, toY);
        }

        public void Promote(SquareNumber square) {
            if (!CanPromote(square)) {
                throw new ManualValidationException("Invalid move.");
            }
            var (x, y) = square.ToPosition(Size);
            _squares[x, y] = _squares[x, y].Promoted();
        }

        public bool CanPromote(SquareNumber square) {
            var color = this[square].Color;
            if (color is null) {
                return false;
            }
            if (this[square].IsKing) {
                return false;
            }
            int y = square.ToPosition(Size).y;
            return color == Color.Black ? y == Size - 1 : y == 0;
        }

        public int NrOfPiecesPerColor(Color color) => All.Count(s => s.Color == color);

        private IEnumerable<Piece> All {
            get {
                for (int y = 0; y < Size; y++) {
                    for (int x = 0; x < Size; x++) {
                        yield return _squares[x, y];
                    }
                }
            }
        }

        public override string ToString() => ToLongString("", "");
        public string ToLongString(string separator = "\n", string empty = " ") {
            var sb = new StringBuilder(_squares.Length + Size);
            for (int y = 0; y < Size; y++) {
                for (int x = 0; x < Size; x++) {
                    sb.Append(IsPlayable(x, y) ? _squares[x, y].RawValue.ToString() : empty);
                }
                if (y != Size - 1) {
                    sb.Append(separator);
                }
            }
            return sb.ToString();
        }

        public static BoardPosition FromString(string input, string separator = "\n", string empty = " ") {
            var chars = input.ToCharArray().Select(c => c.ToString())
                .Where(s => s != empty && s != separator)
                .Select(s => byte.Parse(s))
                .ToArray();
            int size = Convert.ToInt32(Math.Sqrt(chars.Length * 2));
            var squares = new Piece[size, size];
            for (int y = 0; y < size; y++) {
                for (int x = 0; x < size; x++) {
                    squares[x, y] = IsPlayable(x, y)
                        ? new Piece(chars[SquareNumber.FromPosition(x, y, size) - 1])
                        : Piece.Empty;
                }
            }
            return new BoardPosition(squares);
        }

        public static BoardPosition InitialSetup(int boardsize) {
            int nrOfStartingPieces = boardsize * (boardsize - 2) / 4;
            return FromString(new StringBuilder()
                .Append(Piece.BlackMan.ToChar(), nrOfStartingPieces)
                .Append(Piece.Empty.ToChar(), boardsize)
                .Append(Piece.WhiteMan.ToChar(), nrOfStartingPieces)
                .ToString());
        }

        public static bool IsPlayable(int x, int y) => (x + y) % 2 == 1;

        public override bool Equals(object? obj) => Equals(obj as BoardPosition);
        public bool Equals(BoardPosition? other) => other is object && All.SequenceEqual(other.All);

        public override int GetHashCode() => ComparisonUtils.GetHashCode(All);

        public static bool operator ==(BoardPosition? left, BoardPosition? right) => ComparisonUtils.NullSafeEquals(left, right);
        public static bool operator !=(BoardPosition? left, BoardPosition? right) => ComparisonUtils.NullSafeNotEquals(left, right);
    }
}
