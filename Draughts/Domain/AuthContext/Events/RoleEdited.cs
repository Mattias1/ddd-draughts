using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;

namespace Draughts.Domain.AuthContext.Events {
    public class RoleEdited : DomainEvent {
        public const string TYPE = "role.edited";

        public RoleId RoleId { get; }
        public string Rolename { get; }
        public UserId EditedBy { get; }

        public RoleEdited(Role role, UserId editedBy, DomainEventId id, ZonedDateTime created) : base(id, TYPE, created) {
            RoleId = role.Id;
            Rolename = role.Rolename;
            EditedBy = editedBy;
        }

        public static Func<DomainEventId, ZonedDateTime, RoleEdited> Factory(Role role, UserId createdBy) {
            return (id, now) => new RoleEdited(role, createdBy, id, now);
        }
    }
}
