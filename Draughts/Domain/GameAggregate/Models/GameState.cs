using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameAggregate.Models {
    // TODO: This could be it's own aggregate root? Maybe even it's own domain???
    public class GameState : Entity<GameState, GameId> {
        public BoardPosition Board { get; }

        public override GameId Id { get; }

        private GameState(GameId gameId, BoardPosition board) {
            Id = gameId;
            Board = board;
        }

        public string ToStorage() => Board.ToString();

        public static GameState FromStorage(GameId gameId, string hex) => new GameState(gameId, BoardPosition.FromString(hex));

        public static GameState InitialState(GameId gameId, int boardsize) => new GameState(gameId, BoardPosition.InitialSetup(boardsize));
    }
}
