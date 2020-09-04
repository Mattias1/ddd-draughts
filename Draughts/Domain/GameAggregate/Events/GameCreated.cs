using Draughts.Common.Events;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using NodaTime;

namespace Draughts.Domain.GameAggregate.Events {
    public class GameCreated : DomainEvent {
        public const string TYPE = "game.created";

        public GameId GameId { get; }
        public UserId CreatorUserId { get; }

        public GameCreated(Game game, UserId creatorUserId, DomainEventId id, ZonedDateTime created) : base(id, TYPE, created) {
            GameId = game.Id;
            CreatorUserId = creatorUserId;
        }
    }
}
