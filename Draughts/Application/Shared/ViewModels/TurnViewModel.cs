using Draughts.Domain.GameContext.Models;
using NodaTime;

namespace Draughts.Application.Shared.ViewModels {
    public class TurnViewModel {
        public PlayerViewModel Player { get; }
        public ZonedDateTime CreatedAt { get; }
        public ZonedDateTime ExpiresAt { get; }

        public TurnViewModel(Turn turn) {
            Player = new PlayerViewModel(turn.Player);
            CreatedAt = turn.CreatedAt;
            ExpiresAt = turn.ExpiresAt;
        }
    }
}
