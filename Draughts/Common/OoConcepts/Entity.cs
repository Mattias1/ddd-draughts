using Draughts.Common.Utilities;
using System;

namespace Draughts.Common.OoConcepts {
    public abstract class Entity<T, TId> : IEquatable<T> where T : Entity<T, TId> where TId : IdValueObject<TId> {
        public abstract TId Id { get; }

        public override bool Equals(object? obj) => obj is T other && Id.Equals(other.Id);
        public bool Equals(T? other) => Id.Equals(other?.Id);
        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(Entity<T, TId>? left, Entity<T, TId>? right) => ComparisonUtils.NullSafeEquals(left, right);
        public static bool operator !=(Entity<T, TId>? left, Entity<T, TId>? right) => ComparisonUtils.NullSafeNotEquals(left, right);
    }
}
