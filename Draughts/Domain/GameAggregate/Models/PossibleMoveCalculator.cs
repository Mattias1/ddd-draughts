using Draughts.Common;
using System;
using System.Collections.Generic;
using static Draughts.Domain.GameAggregate.Models.GameSettings;

namespace Draughts.Domain.GameAggregate.Models {
    public class PossibleMoveCalculator {
        private BoardPosition _board;
        private GameSettings _settings;
        private Color _currentTurn;
        private SquareId? _restrictedTo;
        private int _minCaptureSequence;

        private bool MustCapture => _minCaptureSequence > 0;

        private PossibleMoveCalculator(BoardPosition board, GameSettings settings,
                Color currentTurn, SquareId? restrictedTo, int minCaptureSequence) {
            _board = board;
            _settings = settings;
            _currentTurn = currentTurn;
            _restrictedTo = restrictedTo;
            _minCaptureSequence = minCaptureSequence;

            if (_restrictedTo is not null && _board[_restrictedTo].Color != _currentTurn) {
                throw new InvalidOperationException("Moves can only be restricted to pieces whose turn it is.");
            }
        }

        public IReadOnlyList<PossibleMove> Calculate() {
            var possibleMoves = new List<PossibleMove>();
            foreach (var from in AllLoopPositions()) {
                foreach (var dir in Direction.All) {
                    if (!_settings.FlyingKings || from.IsMan) {
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
            if (_restrictedTo is not null) {
                yield return _board[_restrictedTo];
            }
            else {
                for (int i = 1; i <= _board.NrOfPlayableSquares; i++) {
                    var squareId = new SquareId(i);
                    if (_board[squareId].Color == _currentTurn) {
                        yield return _board[squareId];
                    }
                }
            }
        }

        private void AddNormalMovesFrom(List<PossibleMove> possibleMoves, Square from, Direction dir) {
            if (!from.TryGetBorder(dir, out var next)) {
                return;
            }

            if (next.Color == _currentTurn.Other) {
                var jump = next.GetBorder(dir);
                if (jump is null || jump.IsNotEmpty || !CanCaptureInDirection(dir, from)) {
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

            if (next.IsEmpty && !MustCapture && CanMoveInDirection(dir, from)) {
                possibleMoves.Add(PossibleMove.NormalMove(from, next));
            }
        }

        private int NormalChainLength(Square from, Square to, Square victim) {
            var capturedPiece = victim.Piece;
            _board.PerformMoveUnsafe(from.Id, to.Id, victim.Id);

            int maxChainLength = 1;
            foreach (var dir in Direction.All) {
                var target = to.GetBorder(dir);
                var jump = target?.GetBorder(dir);
                if (target is null || jump is null) {
                    continue;
                }
                if (target.Color == _currentTurn.Other && jump.IsEmpty && CanCaptureInDirection(dir, to)) {
                    if (_settings.CaptureConstraints == DraughtsCaptureConstraints.AnyFinishedSequence) {
                        maxChainLength = 2;
                        break;
                    }
                    int length = 1 + NormalChainLength(to, jump, target);
                    maxChainLength = Math.Max(maxChainLength, length);
                }
            }

            _board.UndoMoveUnsafe(from.Id, to.Id, victim.Id, capturedPiece);
            return maxChainLength;
        }

        private bool CanCaptureInDirection(Direction dir, Square from) {
            return _settings.MenCaptureBackwards || CanMoveInDirection(dir, from);
        }

        private bool CanMoveInDirection(Direction dir, Square from) {
            return dir.IsForwardsDirection(_currentTurn) || from.IsKing;
        }

        private void AddFlyingMovesFrom(List<PossibleMove> possibleMoves, Square from, Direction dir) {
            Square? victim = null;
            for (var next = from.GetBorder(dir); next is not null; next = next.GetBorder(dir)) {
                if (next.IsEmpty && victim is not null) {
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

                if (next.Color == _currentTurn.Other && victim is null) {
                    var jump = next.GetBorder(dir);
                    if (jump is null || jump.IsNotEmpty) {
                        return;
                    }
                    victim = next;
                    continue;
                }

                if (next.IsEmpty) {
                    if (!MustCapture) {
                        possibleMoves.Add(PossibleMove.NormalMove(from, next));
                    }
                    continue;
                }
                return;
            }
        }

        private int FlyingChainLength(Square from, Square to, Square victim) {
            var capturedPiece = victim.Piece;
            _board.PerformMoveUnsafe(from.Id, to.Id, victim.Id);

            int maxChainLength = 1;
            foreach (var dir in Direction.All) {
                Square? target = FirstNonEmptySquareInDirection(to, dir);
                if (target is null || target.Color != _currentTurn.Other) {
                    continue;
                }

                for (var jump = target.GetBorder(dir); jump is not null && jump.IsEmpty; jump = jump.GetBorder(dir)) {
                    if (_settings.CaptureConstraints == DraughtsCaptureConstraints.AnyFinishedSequence) {
                        maxChainLength = 2;
                        goto end_of_loop;
                    }
                    int length = 1 + FlyingChainLength(to, jump, target);
                    maxChainLength = Math.Max(maxChainLength, length);
                }
            }

        end_of_loop:
            _board.UndoMoveUnsafe(from.Id, to.Id, victim.Id, capturedPiece);
            return maxChainLength;
        }

        private static Square? FirstNonEmptySquareInDirection(Square to, Direction dir) {
            Square? target = to;
            do {
                target = target.GetBorder(dir);
            } while (target is not null && target.IsEmpty);
            return target;
        }

        public static PossibleMoveCalculator ForNewTurn(BoardPosition board, Color currentTurn, GameSettings settings) {
            return new PossibleMoveCalculator(board, settings, currentTurn, null, 0);
        }

        public static PossibleMoveCalculator ForChainCaptures(BoardPosition board, SquareId from, GameSettings settings) {
            var currentTurn = board[from].Color ?? throw new ManualValidationException("Invalid move.");
            return new PossibleMoveCalculator(board, settings, currentTurn, from, 1);
        }
    }
}
