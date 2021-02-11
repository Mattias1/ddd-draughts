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
        public Piece this[Square n] {
            get {
                var (x, y) = n.ToPosition(Size);
                return this[x, y];
            }
        }

        public int Size => _squares.GetLength(0);
        public int NrOfPlayableSquares => Size * Size / 2;

        private BoardPosition(Piece[,] squares) => _squares = squares;

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

            var (fromX, fromY, toX, toY) = ExtractPositions(from, to);
            _squares[toX, toY] = _squares[fromX, fromY];
            _squares[fromX, fromY] = Piece.Empty;
            if (move.Victim != null) {
                var (victimX, victimY) = move.Victim.ToPosition(Size);
                _squares[victimX, victimY] = Piece.Empty;
            }

            canCaptureMore = move.MoreCapturesAvailable;
        }

        private (int, int, int, int) ExtractPositions(Square from, Square to) {
            var (fromX, fromY) = from.ToPosition(Size);
            var (toX, toY) = to.ToPosition(Size);
            return (fromX, fromY, toX, toY);
        }

        public void Promote(Square square) {
            if (!CanPromote(square)) {
                throw new ManualValidationException("Invalid move.");
            }
            var (x, y) = square.ToPosition(Size);
            _squares[x, y] = _squares[x, y].Promoted();
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
                        ? new Piece(chars[Square.FromPosition(x, y, size) - 1])
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

    // TODO: (non-)flying kings, etc.
    public class PossibleMoveCalculator {
        private BoardPosition _board;
        private Color _currentTurn;
        private Square? _restrictedTo;
        private bool _mustCapture;

        private PossibleMoveCalculator(BoardPosition board, Color currentTurn, Square? restrictedTo, bool mustCapture) {
            _board = board;
            _currentTurn = currentTurn;
            _restrictedTo = restrictedTo;
            _mustCapture = mustCapture;

            if (_restrictedTo != null && _board[_restrictedTo].Color != _currentTurn) {
                throw new InvalidOperationException("Moves can only be restricted to pieces whose turn it is.");
            }
        }

        public IReadOnlyList<PossibleMove> Calculate() {
            var possibleMoves = new List<PossibleMove>();
            foreach (var from in AllLoopPositions()) {
                foreach (var dir in Direction.All) {
                    if (_board[from].IsMan) {
                        AddManMovesFrom(possibleMoves, from, dir);
                    }
                    else {
                        AddKingMovesFrom(possibleMoves, from, dir);
                    }
                }
            }
            return possibleMoves.AsReadOnly();
        }

        private IEnumerable<Square> AllLoopPositions() {
            if (_restrictedTo != null) {
                yield return _restrictedTo;
            }
            else {
                for (int y = 0; y < _board.Size; y++) {
                    for (int x = 0; x < _board.Size; x++) {
                        if (_board[x, y].Color == _currentTurn) {
                            yield return Square.FromPosition(x, y, _board.Size);
                        }
                    }
                }
            }
        }

        private void AddManMovesFrom(List<PossibleMove> possibleMoves, Square from, Direction direction) {
            if (!from.TryGetBorder(direction, _board.Size, out Square? next)) {
                return;
            }
            Color? nextColor = _board[next].Color;

            if (nextColor == _currentTurn.Other) {
                if (!next.TryGetBorder(direction, _board.Size, out Square? jump) || _board[jump].IsNotEmpty) {
                    return;
                }
                if (!_mustCapture) {
                    _mustCapture = true;
                    possibleMoves.Clear();
                }
                possibleMoves.Add(new PossibleMove(from, jump, next));
                // TODO: Recurse to determine chain length. If longer, clear all previous moves again :)
                return;
            }

            if (nextColor is null && !_mustCapture && direction.IsForwardsDirection(_currentTurn)) {
                possibleMoves.Add(new PossibleMove(from, next));
            }
        }

        private void AddKingMovesFrom(List<PossibleMove> possibleMoves, Square from, Direction direction) {
            Square? next = from;
            Square? victim = null;
            while(next.TryGetBorder(direction, _board.Size, out next)) {
                Color? nextColor = _board[next].Color;

                if (nextColor is null && victim != null) {
                    // TODO: Recurse to determine chain length. If longer, clear all previous moves again :)
                    possibleMoves.Add(new PossibleMove(from, next, victim, false));
                    continue;
                }

                if (nextColor == _currentTurn.Other || victim != null) {
                    if (!next.TryGetBorder(direction, _board.Size, out Square? jump) || _board[jump].IsNotEmpty) {
                        return;
                    }
                    if (!_mustCapture) {
                        _mustCapture = true;
                        possibleMoves.Clear();
                    }
                    // TODO: Recurse to determine chain length. If longer, clear all previous moves again :)
                    possibleMoves.Add(new PossibleMove(from, jump, next, false));
                    victim = next;
                    next = jump;
                    continue;
                }

                if (nextColor is null) {
                    if (!_mustCapture) {
                        possibleMoves.Add(new PossibleMove(from, next));
                    }
                    continue;
                }
                return;
            }
        }

        public static PossibleMoveCalculator ForNewTurn(BoardPosition board, Color currentTurn) {
            return new PossibleMoveCalculator(board, currentTurn, null, false);
        }
        public static PossibleMoveCalculator ForChainCaptures(BoardPosition board, Square from) {
            var currentTurn = board[from].Color ?? throw new ManualValidationException("Invalid move.");
            return new PossibleMoveCalculator(board, currentTurn, from, true);
        }
    }
}
