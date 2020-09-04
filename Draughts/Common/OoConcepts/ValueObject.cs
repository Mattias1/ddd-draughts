using Draughts.Common.Utilities;
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

    public abstract class IdValueObject<T> : ValueObject<T>, IEquatable<long?>, IComparable<T> where T : IdValueObject<T> {
        public abstract long Id { get; }

        public override string ToString() => Id.ToString();

        public int CompareTo(T? other) => Id.CompareTo(other?.Id);

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return Id;
        }

        public override bool Equals(object? obj) => obj switch
        {
            IdValueObject<T> v => Equals(v),
            long id => Equals(id),
            _ => false
        };
        public bool Equals(IdValueObject<T>? obj) => Id == obj?.Id;
        public bool Equals(long? id) => Id == id;

        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(IdValueObject<T>? left, long? right) => left?.Id == right;
        public static bool operator !=(IdValueObject<T>? left, long? right) => left?.Id != right;
    }

    public abstract class StringValueObject<T> : ValueObject<T>, IEquatable<string>, IComparable<T> where T : StringValueObject<T> {
        public abstract string Value { get; }

        public override string ToString() => Value;

        public int CompareTo(T? other) => Value.CompareTo(other?.Value);

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return Value.ToLower();
        }

        public override bool Equals(object? obj) => obj switch
        {
            StringValueObject<T> v => Equals(v),
            string value => Equals(value),
            _ => false
        };
        public bool Equals(StringValueObject<T>? obj) => Equals(obj?.Value);
        public bool Equals(string? value) => Value.Equals(value, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode() => Value.ToLower().GetHashCode();

        public static bool operator ==(StringValueObject<T>? left, string? right) => ComparisonUtils.EquatableNullSafeEquals(left, right);
        public static bool operator !=(StringValueObject<T>? left, string? right) => ComparisonUtils.EquatableNullSafeNotEquals(left, right);
    }
}
