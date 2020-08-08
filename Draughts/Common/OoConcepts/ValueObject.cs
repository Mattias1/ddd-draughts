using Draughts.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Common.OoConcepts {
    public abstract class ValueObject<T> : IEquatable<T> where T : ValueObject<T> {
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj) => obj is T other && EqualsCore(other);
        public bool Equals(T? other) => other is object && EqualsCore(other);
        protected bool EqualsCore(T other) => GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

        public override int GetHashCode() {
            return GetEqualityComponents().Aggregate(1, (current, obj) => {
                unchecked {
                    return current * 23 + (obj?.GetHashCode() ?? 0);
                }
            });
        }

        public static bool operator ==(ValueObject<T>? left, ValueObject<T>? right) => ComparisonUtils.NullSafeEquals(left, right);
        public static bool operator !=(ValueObject<T>? left, ValueObject<T>? right) => ComparisonUtils.NullSafeNotEquals(left, right);
    }

    public abstract class IdValueObject<T> : ValueObject<T>, IComparable<T> where T : IdValueObject<T> {
        public abstract long Id { get; }

        public override string ToString() => Id.ToString();

        public int CompareTo(T? other) => Id.CompareTo(other?.Id);

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return Id;
        }
    }

#pragma warning disable CS0660, CS0661 // Type defines operator == or operator != but does not override Object.Equals(object o), Object.GetHashCode()
    public abstract class StringValueObject<T> : ValueObject<T>, IComparable<T> where T : StringValueObject<T> {
#pragma warning restore CS0660, CS0661 // Type defines operator == or operator != but does not override Object.Equals(object o), Object.GetHashCode()
        public abstract string Value { get; }

        public override string ToString() => Value;

        public int CompareTo(T? other) => Value.CompareTo(other?.Value);

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return Value.ToLower();
        }

        public static bool operator ==(StringValueObject<T>? left, string? right) => left is null ? right is null : left.Equals(right);
        public static bool operator !=(StringValueObject<T>? left, string? right) => !(left == right);
    }
}
