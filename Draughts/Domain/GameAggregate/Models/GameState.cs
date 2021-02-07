using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameAggregate.Models {
    // TODO: Maybe make this it's own aggregate?
    public class GameState : Entity<GameState, GameId> {
        public enum MoveResult { NextTurn, MoreCapturesAvailable, GameOver };

        public BoardPosition Board { get; }
        public Square? CaptureSequencePreviousSquare { get; private set; }

        public override GameId Id { get; }

        private GameState(GameId gameId, BoardPosition board) {
            Id = gameId;
            Board = board;
        }

        public MoveResult AddMove(Square from, Square to, Color currentTurn) {
            // TODO: Validate if from and to fit on the board
            if (currentTurn != Board[from].Color) {
                throw new ManualValidationException("It's not your turn.");
            }

            bool isNormalMove = Board.IsMove(from, to);
            if (!isNormalMove && !Board.IsCapture(from, to)) {
                throw new ManualValidationException("Invalid move.");
            }

            // TODO: Add the event.

            if (isNormalMove) {
                Board.Move(from, to);
            }
            else {
                Board.Capture(from, to);

                // TODO: If chain capture, return that.
            }

            if (Board.CanPromote(to)) {
                Board.Promote(to);
            }

            if (Board.NrOfPiecesPerColor(currentTurn.Other) == 0) {
                return MoveResult.GameOver;
            }

            return MoveResult.NextTurn;
        }

        public string ToStorage() => Board.ToString();

        public static GameState FromStorage(GameId gameId, string storage) => new GameState(gameId, BoardPosition.FromString(storage));

        public static GameState InitialState(GameId gameId, int boardsize) => new GameState(gameId, BoardPosition.InitialSetup(boardsize));
    }
}
