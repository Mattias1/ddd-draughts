using Draughts.Domain.GameContext.Models;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.Shared.ViewModels {
    public class GameViewModel {
        public GameId Id { get; }
        public IReadOnlyList<PlayerViewModel> Players { get; }
        public Turn? Turn { get; }
        public GameSettings Settings { get; }
        public PlayerViewModel? Victor { get; }
        public ZonedDateTime CreatedAt { get; }
        public ZonedDateTime? StartedAt { get; }
        public ZonedDateTime? FinishedAt { get; }

        public GameViewModel(Game game) {
            Id = game.Id;
            Players = game.Players.Select(p => new PlayerViewModel(p)).ToList().AsReadOnly();
            Turn = game.Turn;
            Settings = game.Settings;
            Victor = game.Victor is null ? null : new PlayerViewModel(game.Victor);
            CreatedAt = game.CreatedAt;
            StartedAt = game.StartedAt;
            FinishedAt = game.FinishedAt;
        }
    }

    public class PlayGameViewModel : GameViewModel {
        public GameStateViewModel CurrentGameState { get; }

        public PlayGameViewModel(Game game, GameState gameState) : base(game) {
            CurrentGameState = new GameStateViewModel(gameState);
        }
    }
}
