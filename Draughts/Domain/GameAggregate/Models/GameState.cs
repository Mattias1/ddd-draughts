using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameAggregate.Models {
    // TODO: Maybe make this it's own aggregate?
    public class GameState : Entity<GameState, GameId> {
        public enum MoveResult { NextTurn, MoreCapturesAvailable, GameOver };

        public BoardPosition Board { get; }
        public Square? CaptureSequenceFrom { get; private set; }

        public override GameId Id { get; }

        private GameState(GameId gameId, BoardPosition board, Square? captureSequenceFrom) {
            Id = gameId;
            Board = board;
            CaptureSequenceFrom = captureSequenceFrom;
        }

        public MoveResult AddMove(Square from, Square to, Color currentTurn) {
            if (from > Board.NrOfPlayableSquares || to > Board.NrOfPlayableSquares) {
                throw new ManualValidationException("Invalid move.");
            }
            if (currentTurn != Board[from].Color) {
                throw new ManualValidationException("It's not your turn.");
            }

            PerformMove(from, to, out bool canCaptureMore);

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

        private void PerformMove(Square from, Square to, out bool canCaptureMore) {
            if (CaptureSequenceFrom is null) {
                Board.PerformNewMove(from, to, out canCaptureMore);
            }
            else {
                if (CaptureSequenceFrom != from) {
                    throw new ManualValidationException("Invalid move, you have to continue the capture sequence.");
                }
                Board.PerformChainCaptureMove(from, to, out canCaptureMore);
            }
        }

        public string StorageString() => Board.ToString();

        public static GameState FromStorage(GameId gameId, string storage, int? fromSquare) {
            var captureSequenceFrom = fromSquare is null ? null : new Square(fromSquare);
            return new GameState(gameId, BoardPosition.FromString(storage), captureSequenceFrom);
        }

        public static GameState InitialState(GameId gameId, int boardsize) {
            return new GameState(gameId, BoardPosition.InitialSetup(boardsize), null);
        }
    }
}
