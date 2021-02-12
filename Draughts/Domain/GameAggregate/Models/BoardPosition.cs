using Draughts.Common;
using Draughts.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Draughts.Domain.GameAggregate.Models {
    // This class is like a mutuable value object. It could be immutable, but that'd be not very performant. Maybe. Hmmm. :/
    public class BoardPosition : IEquatable<BoardPosition> {
        private readonly Piece[] _pieces;
        public int Size { get; }

        /// <summary>
        /// The square at (x, y).
        /// The top left square is (0, 0).
        /// </summary>
        public Piece this[int x, int y] => IsPlayable(x, y) ? this[Square.FromPosition(x, y, Size)] : Piece.Empty;
        public Piece this[Square n] {
            get => _pieces[n.Value - 1];
            private set => _pieces[n.Value - 1] = value;
        }

        public int NrOfPlayableSquares => _pieces.Length;

        private BoardPosition(int size, Piece[] pieces) {
            Size = size;
            _pieces = pieces;
        }

        public void PerformNewMove(Square from, Square to, out bool canCaptureMore) {
            var currentTurn = this[from].Color ?? throw new ManualValidationException("Invalid move.");
            var possibleMoves = PossibleMoveCalculator.ForNewTurn(this, currentTurn).Calculate();
            PerformMove(from, to, possibleMoves, out canCaptureMore);
        }

        public void PerformChainCaptureMove(Square from, Square to, out bool canCaptureMore) {
            var possibleMoves = PossibleMoveCalculator.ForChainCaptures(this, from).Calculate();
            PerformMove(from, to, possibleMoves, out canCaptureMore);
        }

        private void PerformMove(Square from, Square to, IReadOnlyList<PossibleMove> possibleMoves, out bool canCaptureMore) {
            var move = possibleMoves.SingleOrDefault(m => m.From == from && m.To == to);
            if (move is null) {
                throw new ManualValidationException("Invalid move.");
            }

            PerformMoveUnsafe(from, to, move.Victim);
            canCaptureMore = move.MoreCapturesAvailable;
        }

        internal void PerformMoveUnsafe(Square from, Square to, Square? victim) {
            this[to] = this[from];
            this[from] = Piece.Empty;
            if (victim != null) {
                this[victim] = Piece.Empty;
            }
        }

        internal void UndoMoveUnsafe(Square from, Square to, Square? victim, Piece capturedPiece) {
            this[from] = this[to];
            this[to] = Piece.Empty;
            if (victim != null) {
                this[victim] = capturedPiece;
            }
        }

        public void Promote(Square square) {
            if (!CanPromote(square)) {
                throw new ManualValidationException("Invalid move.");
            }
            this[square] = this[square].Promoted();
        }

        public bool CanPromote(Square square) {
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

        public int NrOfPiecesPerColor(Color color) => _pieces.Count(p => p.Color == color);

        public override string ToString() => ToLongString("", "");
        public string ToLongString(string separator = "\n", string empty = " ") {
            var sb = new StringBuilder(_pieces.Length * 2 + Size);
            for (int y = 0; y < Size; y++) {
                for (int x = 0; x < Size; x++) {
                    sb.Append(IsPlayable(x, y) ? this[x, y].RawValue.ToString() : empty);
                }
                if (y != Size - 1) {
                    sb.Append(separator);
                }
            }
            return sb.ToString();
        }

        public static BoardPosition FromString(string input, string separator = "\n", string empty = " ") {
            var pieces = input.ToCharArray().Select(c => c.ToString())
                .Where(s => s != empty && s != separator)
                .Select(s => new Piece(byte.Parse(s)))
                .ToArray();
            int size = Convert.ToInt32(Math.Sqrt(pieces.Length * 2));
            return new BoardPosition(size, pieces);
        }

        public static BoardPosition InitialSetup(int boardsize) {
            int nrOfStartingPieces = boardsize * (boardsize - 2) / 4;
            var pieces = new Piece[nrOfStartingPieces + boardsize + nrOfStartingPieces];
            for (int i = 0; i < nrOfStartingPieces; i++) {
                pieces[i] = Piece.BlackMan;
                pieces[pieces.Length - i - 1] = Piece.WhiteMan;
            }
            for (int i=0; i < boardsize; i++) {
                pieces[i + nrOfStartingPieces] = Piece.Empty;
            }
            return new BoardPosition(boardsize, pieces);
        }

        public static bool IsPlayable(int x, int y) => (x + y) % 2 == 1;

        public override bool Equals(object? obj) => Equals(obj as BoardPosition);
        public bool Equals(BoardPosition? other) => other is object && _pieces.SequenceEqual(other._pieces);

        public override int GetHashCode() => ComparisonUtils.GetHashCode(_pieces);

        public static bool operator ==(BoardPosition? left, BoardPosition? right) => ComparisonUtils.NullSafeEquals(left, right);
        public static bool operator !=(BoardPosition? left, BoardPosition? right) => ComparisonUtils.NullSafeNotEquals(left, right);
    }
}
