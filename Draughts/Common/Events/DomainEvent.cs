using Draughts.Common.Utilities;
using NodaTime;
using System;

namespace Draughts.Common.Events;

public abstract class DomainEvent : IEquatable<DomainEvent> {
    public delegate DomainEvent DomainEventFactory(DomainEventId eventId, ZonedDateTime createdAt);

    public DomainEventId Id { get; }
    public string Type { get; }
    public ZonedDateTime CreatedAt { get; }
    public ZonedDateTime LastAttemptedAt { get; private set; }
    public int NrOfAttempts { get; private set; }

    public DomainEvent(DomainEventId id, string type, ZonedDateTime createdAt) {
        Id = id;
        Type = type;
        CreatedAt = createdAt;
        LastAttemptedAt = createdAt;
        NrOfAttempts = 0;
    }

    public void RegisterFailedAttempt(ZonedDateTime zonedDateTime) {
        LastAttemptedAt = zonedDateTime;
        NrOfAttempts++;
    }

    public override bool Equals(object? obj) => obj is DomainEvent other && Id.Equals(other.Id);
    public bool Equals(DomainEvent? other) => Id.Equals(other?.Id);
    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(DomainEvent? left, DomainEvent? right) => ComparisonUtils.NullSafeEquals(left, right);
    public static bool operator !=(DomainEvent? left, DomainEvent? right) => ComparisonUtils.NullSafeNotEquals(left, right);
}
