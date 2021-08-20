using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;

namespace Draughts.Domain.AuthContext.Events {
    public class UserGainedRole : DomainEvent {
        public const string TYPE = "role.gained";

        public RoleId RoleId { get; }
        public string Rolename { get; }
        public UserId UserId { get; }
        public Username Username { get; }
        public UserId AssignedBy { get; }

        public UserGainedRole(Role role, AuthUser user, UserId assignedBy, DomainEventId id, ZonedDateTime created) : base(id, TYPE, created) {
            RoleId = role.Id;
            Rolename = role.Rolename;
            UserId = user.Id;
            Username = user.Username;
            AssignedBy = assignedBy;
        }

        public static Func<DomainEventId, ZonedDateTime, UserGainedRole> Factory(Role role, AuthUser user, UserId assignedBy) {
            return (id, now) => new UserGainedRole(role, user, assignedBy, id, now);
        }
    }
}
