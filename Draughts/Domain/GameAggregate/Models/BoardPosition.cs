using Draughts.Common;
using Draughts.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        // TODO: (non-)flying kings, etc.
        public class PossibleMoveCalculator {
            private BoardPosition _board;
            private Color _currentTurn;
            private Square? _restrictedTo;
            private int _minCaptureSequence;

            private bool MustCapture => _minCaptureSequence > 0;

            private PossibleMoveCalculator(BoardPosition board, Color currentTurn, Square? restrictedTo, int minCaptureSequence) {
                _board = board;
                _currentTurn = currentTurn;
                _restrictedTo = restrictedTo;
                _minCaptureSequence = minCaptureSequence;

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
                    for (int i = 1; i <= _board.NrOfPlayableSquares; i++) {
                        var square = new Square(i);
                        if (_board[square].Color == _currentTurn) {
                            yield return square;
                        }
                    }
                }
            }

            private void AddManMovesFrom(List<PossibleMove> possibleMoves, Square from, Direction dir) {
                if (!TryGetBorderOf(from, dir, out var next, out var nextColor)) {
                    return;
                }

                if (nextColor == _currentTurn.Other) {
                    if (!TryGetBorderTo(next, dir, out var jump, c => c is null)) {
                        return;
                    }
                    int chainLength = ChainLengthMan(from, jump, next);
                    if (chainLength > _minCaptureSequence) {
                        _minCaptureSequence = chainLength;
                        possibleMoves.Clear();
                    }
                    if (chainLength >= _minCaptureSequence) {
                        possibleMoves.Add(PossibleMove.CaptureMove(from, jump, next, chainLength > 1));
                    }
                    return;
                }

                if (nextColor is null && !MustCapture && dir.IsForwardsDirection(_currentTurn)) {
                    possibleMoves.Add(PossibleMove.NormalMove(from, next));
                }
            }

            private int ChainLengthMan(Square from, Square to, Square victim) {
                var capturedPiece = _board[victim];
                _board.PerformMoveUnsafe(from, to, victim);

                int maxChainLength = 1;
                foreach (var dir in Direction.All) {
                    if (TryGetBorderTo(to, dir, out var target, c => c == _currentTurn.Other)
                            && TryGetBorderTo(target, dir, out var jump, c => c is null)) {
                        int length = 1 + ChainLengthMan(to, jump, target);
                        maxChainLength = Math.Max(maxChainLength, length);
                    }
                }

                _board.UndoMoveUnsafe(from, to, victim, capturedPiece);
                return maxChainLength;
            }

            private void AddKingMovesFrom(List<PossibleMove> possibleMoves, Square from, Direction dir) {
                Square? next = from;
                Square? victim = null;
                while (TryGetBorderOf(next, dir, out next, out var nextColor)) {
                    if (nextColor is null && victim != null) {
                        int chainLength = ChainLengthKing(from, next, victim);
                        if (chainLength > _minCaptureSequence) {
                            _minCaptureSequence = chainLength;
                            possibleMoves.Clear();
                        }
                        if (chainLength >= _minCaptureSequence) {
                            possibleMoves.Add(PossibleMove.CaptureMove(from, next, victim, chainLength > 1));
                        }
                        continue;
                    }

                    if (nextColor == _currentTurn.Other || victim != null) {
                        if (!TryGetBorderTo(next, dir, out var jump, c => c is null)) {
                            return;
                        }
                        int chainLength = ChainLengthKing(from, jump, next);
                        if (chainLength > _minCaptureSequence) {
                            _minCaptureSequence = chainLength;
                            possibleMoves.Clear();
                        }
                        if (chainLength >= _minCaptureSequence) {
                            possibleMoves.Add(PossibleMove.CaptureMove(from, jump, next, chainLength > 1));
                        }
                        victim = next;
                        next = jump;
                        continue;
                    }

                    if (nextColor is null) {
                        if (!MustCapture) {
                            possibleMoves.Add(PossibleMove.NormalMove(from, next));
                        }
                        continue;
                    }
                    return;
                }
            }

            private int ChainLengthKing(Square from, Square to, Square victim) {
                var capturedPiece = _board[victim];
                _board.PerformMoveUnsafe(from, to, victim);

                int maxChainLength = 1;
                foreach (var dir in Direction.All) {
                    Square? target = to;
                    while (TryGetBorderTo(target, dir, out target, c => c is null)) {
                        // Do nothing, just loop until we find an occupied square
                    }
                    if (target is null || _board[target].Color != _currentTurn.Other) {
                        continue;
                    }

                    Square? jump = target;
                    while (TryGetBorderTo(jump, dir, out jump, c => c is null)) {
                        int length = 1 + ChainLengthKing(to, jump, target);
                        maxChainLength = Math.Max(maxChainLength, length);
                    }
                }

                _board.UndoMoveUnsafe(from, to, victim, capturedPiece);
                return maxChainLength;
            }

            private bool TryGetBorderOf(Square origin, Direction dir,
                    [NotNullWhen(returnValue: true)] out Square? border, out Color? color) {
                if (origin.TryGetBorder(dir, _board.Size, out border)) {
                    color = _board[border].Color;
                    return true;
                }
                color = null;
                return false;
            }
            private bool TryGetBorderTo(Square origin, Direction dir,
                    [NotNullWhen(returnValue: true)] out Square? border, Predicate<Color?> predicate) {
                return origin.TryGetBorder(dir, _board.Size, out border) && predicate(_board[border].Color);
            }

            public static PossibleMoveCalculator ForNewTurn(BoardPosition board, Color currentTurn) {
                return new PossibleMoveCalculator(board, currentTurn, null, 0);
            }

            public static PossibleMoveCalculator ForChainCaptures(BoardPosition board, Square from) {
                var currentTurn = board[from].Color ?? throw new ManualValidationException("Invalid move.");
                return new PossibleMoveCalculator(board, currentTurn, from, 1);
            }
        }
    }
}
