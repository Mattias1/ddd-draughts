using Draughts.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Draughts.Domain.GameContext.Models.GameSettings;

namespace Draughts.Domain.GameContext.Models;

public sealed class PossibleMoveCalculator {
    private Board _board;
    private GameSettings _settings;
    private Color _currentTurn;
    private SquareId? _restrictedTo;
    private int _minCaptureSequenceLength;

    private bool MustCapture => _minCaptureSequenceLength > 0;

    private PossibleMoveCalculator(Board board, GameSettings settings,
            Color currentTurn, SquareId? restrictedTo, int minCaptureSequenceLength) {
        _board = board;
        _settings = settings;
        _currentTurn = currentTurn;
        _restrictedTo = restrictedTo;
        _minCaptureSequenceLength = minCaptureSequenceLength;

        if (_restrictedTo is not null && _board[_restrictedTo].ColorOfPiece != _currentTurn) {
            throw new InvalidOperationException("Moves can only be restricted to pieces whose turn it is.");
        }
    }

    public IReadOnlyList<PossibleMove> Calculate() {
        var possibleMoves = new List<PossibleMove>();
        foreach (var from in AllLoopPositions()) {
            foreach (var dir in Direction.All) {
                if (!_settings.FlyingKings || from.HasMan) {
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
                if (_board[squareId].ColorOfPiece == _currentTurn) {
                    yield return _board[squareId];
                }
            }
        }
    }

    private void AddNormalMovesFrom(List<PossibleMove> possibleMoves, Square from, Direction dir) {
        var (next, jump) = from.GetTargetAndJump(dir);
        if (IsKillable(next) && IsFree(jump) && IsValidCaptureDirection(dir, from.Piece)) {
            int chainLength = NormalChainLength(from, jump, next);
            if (chainLength > _minCaptureSequenceLength) {
                _minCaptureSequenceLength = chainLength;
                possibleMoves.Clear();
            }
            if (chainLength >= _minCaptureSequenceLength) {
                possibleMoves.Add(PossibleMove.CaptureMove(from, jump, next, chainLength > 1));
            }
            return;
        }

        if (!MustCapture && IsFree(next) && IsValidMoveDirection(dir, from.Piece)) {
            possibleMoves.Add(PossibleMove.NormalMove(from, next));
        }
    }

    private int NormalChainLength(Square from, Square to, Square victim) {
        var currentPiece = from.Piece;
        var capturedPiece = victim.Piece;
        _board.PerformMoveUnsafe(from.Id, to.Id, victim.Id);

        int maxChainLength = 1;
        foreach (var dir in Direction.All) {
            var (target, jump) = to.GetTargetAndJump(dir);
            if (IsKillable(target) && IsFree(jump) && IsValidCaptureDirection(dir, currentPiece)) {
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

    private bool IsKillable([NotNullWhen(returnValue: true)] Square? target) {
        return target is not null && target.ColorOfPiece == _currentTurn.Other && target.HasLivingPiece;
    }

    private bool IsFree([NotNullWhen(returnValue: true)] Square? jump) {
        return jump is not null && jump.IsEmpty;
    }

    private bool IsValidCaptureDirection(Direction dir, Piece currentPiece) {
        return _settings.MenCaptureBackwards || IsValidMoveDirection(dir, currentPiece);
    }

    private bool IsValidMoveDirection(Direction dir, Piece currentPiece) {
        return dir.IsForwardsDirection(_currentTurn) || currentPiece.IsKing;
    }

    private void AddFlyingMovesFrom(List<PossibleMove> possibleMoves, Square from, Direction dir) {
        Square? victim = null;
        for (var next = from.GetBorder(dir); next is not null; next = next.GetBorder(dir)) {
            if (IsFree(next) && HasVictim(victim)) {
                int chainLength = FlyingChainLength(from, next, victim);
                if (chainLength > _minCaptureSequenceLength) {
                    _minCaptureSequenceLength = chainLength;
                    possibleMoves.Clear();
                }
                if (chainLength >= _minCaptureSequenceLength) {
                    possibleMoves.Add(PossibleMove.CaptureMove(from, next, victim, chainLength > 1));
                }
                continue;
            }

            if (IsKillable(next) && HasNoVictimYet(victim)) {
                var jump = next.GetBorder(dir);
                if (IsFree(jump)) {
                    victim = next;
                    continue;
                }
                return;
            }

            if (IsFree(next)) {
                if (!MustCapture) {
                    possibleMoves.Add(PossibleMove.NormalMove(from, next));
                }
                continue;
            }
            return;
        }
    }

    private bool HasVictim([NotNullWhen(returnValue: true)] Square? victim) => victim is not null;
    private bool HasNoVictimYet(Square? victim) => victim is null;

    private int FlyingChainLength(Square from, Square to, Square victim) {
        var capturedPiece = victim.Piece;
        _board.PerformMoveUnsafe(from.Id, to.Id, victim.Id);

        int maxChainLength = 1;
        foreach (var dir in Direction.All) {
            Square? target = FirstNonEmptySquareInDirection(to, dir);
            if (!IsKillable(target)) {
                continue;
            }

            for (var jump = target.GetBorder(dir); IsFree(jump); jump = jump.GetBorder(dir)) {
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

    public static PossibleMoveCalculator ForNewTurn(Board board, Color currentTurn, GameSettings settings) {
        return new PossibleMoveCalculator(board, settings, currentTurn, null, 0);
    }

    public static PossibleMoveCalculator ForChainCaptures(Board board, SquareId from, GameSettings settings) {
        var currentTurn = board[from].ColorOfPiece ?? throw new ManualValidationException("Invalid move.");
        return new PossibleMoveCalculator(board, settings, currentTurn, from, 1);
    }
}
