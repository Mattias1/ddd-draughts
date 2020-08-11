using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using NodaTime;

namespace Draughts.Domain.AuthUserAggregate.Events {
    public class RoleCreated : DomainEvent {
        public const string TYPE = "role.created";

        public RoleId RoleId { get; }
        public string Rolename { get; }

        public RoleCreated(Role role, DomainEventId id, ZonedDateTime created) : base(id, TYPE, created) {
            RoleId = role.Id;
            Rolename = role.Rolename;
        }
    }
}
