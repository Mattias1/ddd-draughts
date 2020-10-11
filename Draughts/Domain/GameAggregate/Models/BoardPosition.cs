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

        public int Width => _squares.GetLength(0);
        public int Height => _squares.GetLength(1);

        private BoardPosition(Piece[,] squares) => _squares = squares;

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
        public string ToLongString(string empty = " ", string separator = "\n") {
            var sb = new StringBuilder(_squares.Length + Height);
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

        public static BoardPosition FromString(string input, string empty = " ", string separator = "\n") {
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
                .Append(Piece.BlackPiece.ToChar(), nrOfStartingPieces)
                .Append(Piece.Empty.ToChar(), boardsize)
                .Append(Piece.WhitePiece.ToChar(), nrOfStartingPieces)
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
