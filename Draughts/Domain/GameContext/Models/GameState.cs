using Draughts.Common;
using Draughts.Common.OoConcepts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.GameContext.Models {
    public class GameState : Entity<GameState, GameId> {
        public const string ERROR_INVALID_SQUARES = "Invalid squares.";
        public const string ERROR_CAPTURE_SEQUENCE = "Continue the capture sequence.";

        public enum MoveResult { NextTurn, MoreCapturesAvailable, GameOver };

        public override GameId Id { get; }

        private List<Move> _moves;

        public Board? _initialBoard;
        public Board Board { get; }
        public SquareId? CaptureSequenceFrom { get; private set; }

        public IReadOnlyList<Move> Moves => _moves.AsReadOnly();

        private GameState(GameId gameId, Board? initialBoard, List<Move> moves, Board board, SquareId? captureSequenceFrom) {
            Id = gameId;
            _initialBoard = initialBoard;
            _moves = moves;
            Board = board;
            CaptureSequenceFrom = captureSequenceFrom;
        }

        public MoveResult AddMove(SquareId from, SquareId to, Color currentTurn, GameSettings settings) {
            if (from.Value > Board.NrOfPlayableSquares || to.Value > Board.NrOfPlayableSquares) {
                throw new ManualValidationException(ERROR_INVALID_SQUARES);
            }
            if (currentTurn != Board[from].Color) {
                throw new ManualValidationException($"You can only move {currentTurn} pieces.");
            }

            PerformMove(from, to, settings, out bool canCaptureMore);

            if (canCaptureMore) {
                CaptureSequenceFrom = to;
                return MoveResult.MoreCapturesAvailable;
            }
            CaptureSequenceFrom = null;

            if (Board.NrOfPiecesPerColor(currentTurn.Other) == 0) {
                return MoveResult.GameOver;
            }

            return MoveResult.NextTurn;
        }

        private void PerformMove(SquareId from, SquareId to, GameSettings settings, out bool canCaptureMore) {
            Move move;
            if (CaptureSequenceFrom is null) {
                move = Board.PerformNewMove(from, to, settings, out canCaptureMore);
            }
            else {
                if (CaptureSequenceFrom != from) {
                    throw new ManualValidationException(ERROR_CAPTURE_SEQUENCE);
                }
                move = Board.PerformChainCaptureMove(from, to, settings, out canCaptureMore);
            }
            _moves.Add(move);
        }

        public string? InitialStateStorageString() => _initialBoard?.ToString();

        public static GameState FromStorage(GameId gameId, GameSettings settings, string? storage, IEnumerable<Move> moves) {
            var board = storage is null ? Board.InitialSetup(settings.BoardSize) : Board.FromString(storage);
            var initialBoard = storage is null ? null : board.Copy();
            SquareId? captureSequenceFrom = PerformAllMoves(board, moves, settings);

            return new GameState(gameId, initialBoard, moves.ToList(), board, captureSequenceFrom);
        }

        private static SquareId? PerformAllMoves(Board board, IEnumerable<Move> moves, GameSettings settings) {
            SquareId? captureSequenceFrom = null;
            foreach (var move in moves) {
                bool canCaptureMore;
                if (captureSequenceFrom is null) {
                    board.PerformNewMove(move.From, move.To, settings, out canCaptureMore);
                }
                else {
                    board.PerformChainCaptureMove(move.From, move.To, settings, out canCaptureMore);
                }
                captureSequenceFrom = canCaptureMore ? move.To : null;
            }

            return captureSequenceFrom;
        }

        public static GameState InitialState(GameId gameId, int boardSize) {
            return new GameState(gameId, null, new List<Move>(), Board.InitialSetup(boardSize), null);
        }
    }
}
