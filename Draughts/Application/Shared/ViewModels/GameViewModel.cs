using Draughts.Domain.GameAggregate.Models;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.Shared.ViewModels {
    public class GameViewModel {
        public GameId Id { get; }
        public IReadOnlyList<PlayerViewModel> Players { get; }
        public Turn? Turn { get; }
        public GameSettings Settings { get; }
        public ZonedDateTime CreatedAt { get; }
        public ZonedDateTime? StartedAt { get; }
        public ZonedDateTime? FinishedAt { get; }

        public GameViewModel(Game game) {
            Id = game.Id;
            Players = game.Players.Select(p => new PlayerViewModel(p)).ToList().AsReadOnly();
            Turn = game.Turn;
            Settings = game.Settings;
            CreatedAt = game.CreatedAt;
            StartedAt = game.StartedAt;
            FinishedAt = game.FinishedAt;
        }
    }
}
