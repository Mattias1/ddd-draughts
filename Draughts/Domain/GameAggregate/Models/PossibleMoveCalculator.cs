using Draughts.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Draughts.Domain.GameAggregate.Models.GameSettings;

namespace Draughts.Domain.GameAggregate.Models {
    public class PossibleMoveCalculator {
        private BoardPosition _board;
        private GameSettings _settings;
        private Color _currentTurn;
        private Square? _restrictedTo;
        private int _minCaptureSequence;

        private bool MustCapture => _minCaptureSequence > 0;

        private PossibleMoveCalculator(BoardPosition board, GameSettings settings,
                Color currentTurn, Square? restrictedTo, int minCaptureSequence) {
            _board = board;
            _settings = settings;
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
                    if (!_settings.FlyingKings || _board[from].IsMan) {
                        AddNormalMovesFrom(possibleMoves, from, dir);
                    }
                    else {
                        AddFlyingMovesFrom(possibleMoves, from, dir);
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

        private void AddNormalMovesFrom(List<PossibleMove> possibleMoves, Square from, Direction dir) {
            if (!TryGetBorderOf(from, dir, out var next, out var nextColor)) {
                return;
            }

            if (nextColor == _currentTurn.Other) {
                if (!CanCaptureInDirection(dir, from) || !TryGetBorderTo(next, dir, out var jump, c => c is null)) {
                    return;
                }
                int chainLength = NormalChainLength(from, jump, next);
                if (chainLength > _minCaptureSequence) {
                    _minCaptureSequence = chainLength;
                    possibleMoves.Clear();
                }
                if (chainLength >= _minCaptureSequence) {
                    possibleMoves.Add(PossibleMove.CaptureMove(from, jump, next, chainLength > 1));
                }
                return;
            }

            if (nextColor is null && !MustCapture && CanMoveInDirection(dir, from)) {
                possibleMoves.Add(PossibleMove.NormalMove(from, next));
            }
        }

        private int NormalChainLength(Square from, Square to, Square victim) {
            var capturedPiece = _board[victim];
            _board.PerformMoveUnsafe(from, to, victim);

            int maxChainLength = 1;
            foreach (var dir in Direction.All) {
                if (CanCaptureInDirection(dir, to)
                        && TryGetBorderTo(to, dir, out var target, c => c == _currentTurn.Other)
                        && TryGetBorderTo(target, dir, out var jump, c => c is null)) {
                    if (_settings.CaptureConstraints == DraughtsCaptureConstraints.AnyFinishedSequence) {
                        maxChainLength = 2;
                        break;
                    }
                    int length = 1 + NormalChainLength(to, jump, target);
                    maxChainLength = Math.Max(maxChainLength, length);
                }
            }

            _board.UndoMoveUnsafe(from, to, victim, capturedPiece);
            return maxChainLength;
        }

        private bool CanCaptureInDirection(Direction dir, Square from) {
            return _settings.MenCaptureBackwards || CanMoveInDirection(dir, from);
        }

        private bool CanMoveInDirection(Direction dir, Square from) {
            return dir.IsForwardsDirection(_currentTurn) || _board[from].IsKing;
        }

        private void AddFlyingMovesFrom(List<PossibleMove> possibleMoves, Square from, Direction dir) {
            Square? next = from;
            Square? victim = null;
            while (TryGetBorderOf(next, dir, out next, out var nextColor)) {
                if (nextColor is null && victim != null) {
                    int chainLength = FlyingChainLength(from, next, victim);
                    if (chainLength > _minCaptureSequence) {
                        _minCaptureSequence = chainLength;
                        possibleMoves.Clear();
                    }
                    if (chainLength >= _minCaptureSequence) {
                        possibleMoves.Add(PossibleMove.CaptureMove(from, next, victim, chainLength > 1));
                    }
                    continue;
                }

                if (nextColor == _currentTurn.Other && victim is null) {
                    if (!TryGetBorderTo(next, dir, out var jump, c => c is null)) {
                        return;
                    }
                    int chainLength = FlyingChainLength(from, jump, next);
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

        private int FlyingChainLength(Square from, Square to, Square victim) {
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
                    if (_settings.CaptureConstraints == DraughtsCaptureConstraints.AnyFinishedSequence) {
                        maxChainLength = 2;
                        goto end_of_loop;
                    }
                    int length = 1 + FlyingChainLength(to, jump, target);
                    maxChainLength = Math.Max(maxChainLength, length);
                }
            }

        end_of_loop:
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

        public static PossibleMoveCalculator ForNewTurn(BoardPosition board, Color currentTurn, GameSettings settings) {
            return new PossibleMoveCalculator(board, settings, currentTurn, null, 0);
        }

        public static PossibleMoveCalculator ForChainCaptures(BoardPosition board, Square from, GameSettings settings) {
            var currentTurn = board[from].Color ?? throw new ManualValidationException("Invalid move.");
            return new PossibleMoveCalculator(board, settings, currentTurn, from, 1);
        }
    }
}