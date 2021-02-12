using Draughts.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Draughts.Domain.GameAggregate.Models {
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