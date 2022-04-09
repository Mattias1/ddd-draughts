using Draughts.Common.OoConcepts;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Domain.GameContext.Models;

public sealed class Turn : ValueObject<Turn> {
    public Player Player { get; }
    public ZonedDateTime CreatedAt { get; }
    public ZonedDateTime ExpiresAt { get; }

    public Turn(Player player, ZonedDateTime createdAt, Duration maxTurnLength)
        : this(player, createdAt, createdAt.Plus(maxTurnLength)) { }
    private Turn(Player player, ZonedDateTime createdAt, ZonedDateTime expiresAt) {
        Player = player;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    protected override IEnumerable<object> GetEqualityComponents() {
        yield return Player;
        yield return CreatedAt;
        yield return ExpiresAt;
    }

    public Turn WithExpiry(ZonedDateTime newExpiresAt) {
        return new Turn(Player, CreatedAt, newExpiresAt);
    }
}
