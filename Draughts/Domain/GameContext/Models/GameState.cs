using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameContext.Models {
    public class GameState : Entity<GameState, GameId> {
        public const string ERROR_INVALID_SQUARES = "Invalid squares.";
        public const string ERROR_CAPTURE_SEQUENCE = "Continue the capture sequence.";

        public enum MoveResult { NextTurn, MoreCapturesAvailable, GameOver };

        public override GameId Id { get; }

        public BoardPosition Board { get; }
        public SquareId? CaptureSequenceFrom { get; private set; }

        private GameState(GameId gameId, BoardPosition board, SquareId? captureSequenceFrom) {
            Id = gameId;
            Board = board;
            CaptureSequenceFrom = captureSequenceFrom;
        }

        public MoveResult AddMove(SquareId from, SquareId to, Color currentTurn, GameSettings settings) {
            if (from > Board.NrOfPlayableSquares || to > Board.NrOfPlayableSquares) {
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

            if (Board.CanPromote(to)) {
                Board.Promote(to);
            }

            if (Board.NrOfPiecesPerColor(currentTurn.Other) == 0) {
                return MoveResult.GameOver;
            }

            return MoveResult.NextTurn;
        }

        private void PerformMove(SquareId from, SquareId to, GameSettings settings, out bool canCaptureMore) {
            if (CaptureSequenceFrom is null) {
                Board.PerformNewMove(from, to, settings, out canCaptureMore);
            }
            else {
                if (CaptureSequenceFrom != from) {
                    throw new ManualValidationException(ERROR_CAPTURE_SEQUENCE);
                }
                Board.PerformChainCaptureMove(from, to, settings, out canCaptureMore);
            }
        }

        public string StorageString() => Board.ToString();

        public static GameState FromStorage(GameId gameId, string storage, int? captureFromSquare) {
            var captureSequenceFrom = captureFromSquare is null ? null : new SquareId(captureFromSquare);
            return new GameState(gameId, BoardPosition.FromString(storage), captureSequenceFrom);
        }

        public static GameState InitialState(GameId gameId, int boardSize) {
            return new GameState(gameId, BoardPosition.InitialSetup(boardSize), null);
        }
    }
}
