using Draughts.Common.Utilities;
using NodaTime;
using System;

namespace Draughts.Common.Events;

public abstract class DomainEvent : IEquatable<DomainEvent> {
    public delegate DomainEvent DomainEventFactory(DomainEventId eventId, ZonedDateTime createdAt);

    public DomainEventId Id { get; }
    public string Type { get; }
    public ZonedDateTime CreatedAt { get; }
    public ZonedDateTime? HandledAt { get; private set; }

    public DomainEvent(DomainEventId id, string type, ZonedDateTime createdAt, ZonedDateTime? handledAt) {
        Id = id;
        Type = type;
        CreatedAt = createdAt;
        HandledAt = handledAt;
    }

    public abstract string BuildDataString();

    public override bool Equals(object? obj) => obj is DomainEvent other && Id.Equals(other.Id);
    public bool Equals(DomainEvent? other) => Id.Equals(other?.Id);
    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(DomainEvent? left, DomainEvent? right) => ComparisonUtils.NullSafeEquals(left, right);
    public static bool operator !=(DomainEvent? left, DomainEvent? right) => ComparisonUtils.NullSafeNotEquals(left, right);
}
