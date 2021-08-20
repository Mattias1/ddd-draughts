using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;

namespace Draughts.Domain.AuthContext.Events {
    public class UserLostRole : DomainEvent {
        public const string TYPE = "role.lost";

        public RoleId RoleId { get; }
        public string Rolename { get; }
        public UserId UserId { get; }
        public Username Username { get; }
        public UserId RemovedBy { get; }

        public UserLostRole(Role role, AuthUser user, UserId removedBy, DomainEventId id, ZonedDateTime created) : base(id, TYPE, created) {
            RoleId = role.Id;
            Rolename = role.Rolename;
            UserId = user.Id;
            Username = user.Username;
            RemovedBy = removedBy;
        }

        public static Func<DomainEventId, ZonedDateTime, UserLostRole> Factory(Role role, AuthUser user, UserId removedBy) {
            return (id, now) => new UserLostRole(role, user, removedBy, id, now);
        }
    }
}
