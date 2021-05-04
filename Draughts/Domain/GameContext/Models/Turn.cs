using Draughts.Common.OoConcepts;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Domain.GameContext.Models {
    public class Turn : ValueObject<Turn> {
        public Player Player { get; }
        public ZonedDateTime CreatedAt { get; }
        public ZonedDateTime ExpiresAt { get; }

        public Turn(Player player, ZonedDateTime createdAt, Duration maxTurnLength) {
            Player = player;
            CreatedAt = createdAt;
            ExpiresAt = createdAt.Plus(maxTurnLength);
        }

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return Player;
            yield return CreatedAt;
            yield return ExpiresAt;
        }
    }
}
