using Draughts.Common;
using Draughts.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Draughts.Domain.GameContext.Models {
    // This class is like a mutuable value object. It could be immutable, but that'd be not very performant. Maybe. Hmmm. :/
    public class BoardPosition : IEquatable<BoardPosition> {
        private readonly Square[] _squares;
        public int Size { get; }

        /// <summary>
        /// The square at (x, y).
        /// The top left square is (0, 0).
        /// </summary>
        public Square? this[int x, int y] => IsPlayable(x, y) ? this[SquareId.FromPosition(x, y, Size)] : null;
        public Square this[SquareId n] {
            get => _squares[n.Value - 1];
            private set => _squares[n.Value - 1] = value;
        }

        public int NrOfPlayableSquares => _squares.Length;

        private BoardPosition(int size, Piece[] pieces) {
            Size = size;
            var squares = new Square[pieces.Length];
            for (int i = 0; i < squares.Length; i++) {
                squares[i] = new Square(new SquareId(i + 1), pieces[i], this);
            }
            _squares = squares;
        }

        public void PerformNewMove(SquareId from, SquareId to, GameSettings settings,out bool canCaptureMore) {
            var currentTurn = this[from].Color ?? throw new ManualValidationException("Invalid move.");
            var possibleMoves = PossibleMoveCalculator.ForNewTurn(this, currentTurn, settings).Calculate();
            PerformMove(from, to, possibleMoves, out canCaptureMore);
        }

        public void PerformChainCaptureMove(SquareId from, SquareId to, GameSettings settings, out bool canCaptureMore) {
            var possibleMoves = PossibleMoveCalculator.ForChainCaptures(this, from, settings).Calculate();
            PerformMove(from, to, possibleMoves, out canCaptureMore);
        }

        private void PerformMove(SquareId from, SquareId to, IReadOnlyList<PossibleMove> possibleMoves, out bool canCaptureMore) {
            var move = possibleMoves.SingleOrDefault(m => m.From == from && m.To == to);
            if (move is null) {
                throw new ManualValidationException("Invalid move.");
            }

            PerformMoveUnsafe(from, to, move.Victim);
            canCaptureMore = move.MoreCapturesAvailable;
        }

        internal void PerformMoveUnsafe(SquareId from, SquareId to, SquareId? victim) {
            this[to].Piece = this[from].Piece;
            this[from].Piece = Piece.Empty;
            if (victim is not null) {
                this[victim].Piece = Piece.Empty;
            }
        }

        internal void UndoMoveUnsafe(SquareId from, SquareId to, SquareId? victim, Piece capturedPiece) {
            this[from].Piece = this[to].Piece;
            this[to].Piece = Piece.Empty;
            if (victim is not null) {
                this[victim].Piece = capturedPiece;
            }
        }

        public void Promote(SquareId square) {
            if (!CanPromote(square)) {
                throw new ManualValidationException("Invalid move.");
            }
            this[square].Piece = this[square].Piece.Promoted();
        }

        public bool CanPromote(SquareId squareId) {
            var square = this[squareId];
            if (square.Color is null || square.IsKing) {
                return false;
            }
            int y = square.ToPosition().y;
            return square.Color == Color.Black ? y == Size - 1 : y == 0;
        }

        public int NrOfPiecesPerColor(Color color) => _squares.Count(p => p.Color == color);

        public override string ToString() => ToLongString("", "");
        public string ToLongString(string separator = "\n", string empty = " ") {
            var sb = new StringBuilder(_squares.Length * 2 + Size);
            for (int y = 0; y < Size; y++) {
                for (int x = 0; x < Size; x++) {
                    sb.Append(this[x, y]?.Piece.RawValue.ToString() ?? empty);
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

        public static BoardPosition InitialSetup(int boardSize) {
            int nrOfStartingPieces = boardSize * (boardSize - 2) / 4;
            var pieces = new Piece[nrOfStartingPieces + boardSize + nrOfStartingPieces];
            for (int i = 0; i < nrOfStartingPieces; i++) {
                pieces[i] = Piece.BlackMan;
                pieces[pieces.Length - i - 1] = Piece.WhiteMan;
            }
            for (int i = 0; i < boardSize; i++) {
                pieces[i + nrOfStartingPieces] = Piece.Empty;
            }
            return new BoardPosition(boardSize, pieces);
        }

        public static bool IsPlayable(int x, int y) => (x + y) % 2 == 1;

        public override bool Equals(object? obj) => Equals(obj as BoardPosition);
        public bool Equals(BoardPosition? other) {
            return other is not null && _squares.Select(s => s.Piece).SequenceEqual(other._squares.Select(s => s.Piece));
        }

        public override int GetHashCode() => ComparisonUtils.GetHashCode(_squares);

        public static bool operator ==(BoardPosition? left, BoardPosition? right) => ComparisonUtils.NullSafeEquals(left, right);
        public static bool operator !=(BoardPosition? left, BoardPosition? right) => ComparisonUtils.NullSafeNotEquals(left, right);
    }
}
