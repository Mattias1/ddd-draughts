using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameAggregate.Models {
    // TODO: Maybe make this it's own aggregate?
    public class GameState : Entity<GameState, GameId> {
        public enum MoveResult { NextTurn, MoreCapturesAvailable, GameOver };

        public BoardPosition Board { get; }

        public override GameId Id { get; }

        private GameState(GameId gameId, BoardPosition board) {
            Id = gameId;
            Board = board;
        }

        public MoveResult AddMove(SquareNumber from, SquareNumber to) {
            // TODO: Validate if from and to fit on the board
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
            }

            // TODO: Promote if necessary

            return MoveResult.NextTurn; // TODO
        }

        public string ToStorage() => Board.ToString();

        public static GameState FromStorage(GameId gameId, string storage) => new GameState(gameId, BoardPosition.FromString(storage));

        public static GameState InitialState(GameId gameId, int boardsize) => new GameState(gameId, BoardPosition.InitialSetup(boardsize));
    }
}
