using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using NodaTime;

namespace Draughts.Domain.GameAggregate.Models {
    public class Player : Entity<Player, PlayerId> {
        public override PlayerId Id { get; }
        public UserId UserId { get; }
        public Username Username { get; }
        public Rank Rank { get; }
        public Color Color { get; }
        public ZonedDateTime CreatedAt { get; }

        public Player(PlayerId id, UserId userId, Username username, Rank rank, Color color, ZonedDateTime createdAt) {
            Id = id;
            UserId = userId;
            Username = username;
            Rank = rank;
            Color = color;
            CreatedAt = createdAt;
        }
    }
}
