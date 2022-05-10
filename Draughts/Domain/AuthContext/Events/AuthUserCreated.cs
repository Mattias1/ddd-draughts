using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;

namespace Draughts.Domain.AuthContext.Events;

public sealed class AuthUserCreated : DomainEvent {
    public const string TYPE = "authuser.created";

    public UserId UserId { get; }
    public Username Username { get; }

    public AuthUserCreated(AuthUser authUser, DomainEventId id, ZonedDateTime created) : base(id, TYPE, created) {
        UserId = authUser.Id;
        Username = authUser.Username;
    }

    public static DomainEventFactory Factory(AuthUser authUser) {
        return (id, now) => new AuthUserCreated(authUser, id, now);
    }
}
