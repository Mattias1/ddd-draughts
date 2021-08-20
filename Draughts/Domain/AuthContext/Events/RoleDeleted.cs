using Draughts.Common.Events;
using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;

namespace Draughts.Domain.AuthUserContext.Events {
    public class RoleDeleted : DomainEvent {
        public const string TYPE = "role.deleted";

        public RoleId RoleId { get; }
        public string Rolename { get; }
        public UserId DeletedBy { get; }

        public RoleDeleted(Role role, UserId deletedBy, DomainEventId id, ZonedDateTime created) : base(id, TYPE, created) {
            RoleId = role.Id;
            Rolename = role.Rolename;
            DeletedBy = deletedBy;
        }

        public static Func<DomainEventId, ZonedDateTime, RoleDeleted> Factory(Role role, UserId createdBy) {
            return (id, now) => new RoleDeleted(role, createdBy, id, now);
        }
    }
}
