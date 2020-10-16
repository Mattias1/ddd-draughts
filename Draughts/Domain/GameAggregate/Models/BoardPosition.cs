using Draughts.Common;
using Draughts.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public int Width => _squares.GetLength(0);
        public int Height => _squares.GetLength(1);

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
            var (victimX, victimY) = FirstNonEmptySpotBetween(from, to)!.ToPosition(Size);
            _squares[toX, toY] = _squares[fromX, fromY];
            _squares[fromX, fromY] = Piece.Empty;
            _squares[victimX, victimY] = Piece.Empty;
        }

        // TODO: (non-)flying kings, moving backwards, etc.
        public bool IsMove(SquareNumber from, SquareNumber to) {
            return this[from].IsNotEmpty && this[to].IsEmpty && DiagonalDistance(from, to) == 1;
        }

        public bool IsCapture(SquareNumber from, SquareNumber to) {
            int distance = DiagonalDistance(from, to);
            if (this[from].IsEmpty || this[to].IsNotEmpty || distance < 2 || distance > 2 && this[from].IsMan) {
                return false;
            }
            // TODO: Make sure you only jump one piece.
            var pos = FirstNonEmptySpotBetween(from, to);
            return pos != null && this[pos].Color != this[from].Color;
        }

        // TODO: This should either not throw (but then return a boolean???) or the diagonal part should be checked separately.
        private int DiagonalDistance(SquareNumber from, SquareNumber to) {
            var (fromX, fromY, toX, toY) = ExtractPositions(from, to);
            int distance = Math.Abs(fromX - toX);
            if (Math.Abs(fromY - toY) != distance) {
                throw new ManualValidationException("Horizontal and vertical distances are different, every move should be along a diagonal.");
            }
            return distance;
        }

        private SquareNumber? FirstNonEmptySpotBetween(SquareNumber from, SquareNumber to) {
            // Assumes from and to are on a diagonal
            var (fromX, fromY, toX, _) = ExtractPositions(from, to);
            var (dirX, dirY) = Direction(from, to);
            var (x, y) = (fromX + dirX, fromY + dirY);
            while (x != toX) {
                if (this[x, y].IsNotEmpty) {
                    return SquareNumber.FromPosition(x, y, Size);
                }
            }
            return null;
        }

        private (int, int) Direction(SquareNumber from, SquareNumber to) {
            // Assumes from and to are on a diagonal
            var (fromX, fromY, toX, toY) = ExtractPositions(from, to);
            return (fromX > toX ? -1 : 1, fromY > toY ? -1 : 1);
        }

        private (int, int, int, int) ExtractPositions(SquareNumber from, SquareNumber to) {
            var (fromX, fromY) = from.ToPosition(Size);
            var (toX, toY) = to.ToPosition(Size);
            return (fromX, fromY, toX, toY);
        }

        public int NrOfPiecesPerColor(Color color) => All.Count(s => s.Color == color);

        private IEnumerable<Piece> All {
            get {
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        yield return _squares[x, y];
                    }
                }
            }
        }

        public override string ToString() => ToLongString("", "");
        public string ToLongString(string separator = "\n", string empty = " ") {
            var sb = new StringBuilder(_squares.Length + Size);
            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    sb.Append(IsPlayable(x, y) ? _squares[x, y].RawValue.ToString() : empty);
                }
                if (y != Height - 1) {
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
