using Draughts.Common.Utilities;
using Draughts.Domain.GameContext.Models;
using NodaTime;

namespace Draughts.Application.Shared.ViewModels;

public class TurnViewModel {
    public PlayerViewModel Player { get; }
    public ZonedDateTime CreatedAt { get; }
    public ZonedDateTime ExpiresAt { get; }
    public ZonedDateTime CurrentTime { get; }

    public TurnViewModel(Turn turn, IClock clock) {
        Player = new PlayerViewModel(turn.Player);
        CreatedAt = turn.CreatedAt;
        ExpiresAt = turn.ExpiresAt;
        CurrentTime = clock.UtcNow();
    }
}
