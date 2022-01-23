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

        public override int GetHashCode() => ComparisonUtils.GetHashCode(GetEqualityComponents());

        public static bool operator ==(ValueObject<T>? left, ValueObject<T>? right) => ComparisonUtils.NullSafeEquals(left, right);
        public static bool operator !=(ValueObject<T>? left, ValueObject<T>? right) => ComparisonUtils.NullSafeNotEquals(left, right);
    }

    public abstract class IdValueObject<T> : ValueObject<T>, IEquatable<long?>, IComparable<T> where T : IdValueObject<T> {
        public abstract long Value { get; }

        public override string ToString() => Value.ToString();

        public int CompareTo(T? other) => Value.CompareTo(other?.Value);

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return Value;
        }

        public override bool Equals(object? obj) => obj switch {
            IdValueObject<T> v => Equals(v),
            long id => Equals(id),
            _ => false
        };
        public bool Equals(IdValueObject<T>? obj) => Value == obj?.Value;
        public bool Equals(long? id) => Value == id;

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(IdValueObject<T>? left, long? right) => left?.Value == right;
        public static bool operator !=(IdValueObject<T>? left, long? right) => left?.Value != right;
    }

    public abstract class IntValueObject<T> : ValueObject<T>, IEquatable<int?>, IComparable<T> where T : IntValueObject<T> {
        public abstract int Value { get; }

        public override string ToString() => Value.ToString();

        public int CompareTo(T? other) => Value.CompareTo(other?.Value);

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return Value;
        }

        public override bool Equals(object? obj) => obj switch {
            IntValueObject<T> v => Equals(v),
            int value => Equals(value),
            _ => false
        };
        public bool Equals(IntValueObject<T>? obj) => Value == obj?.Value;
        public bool Equals(int? id) => Value == id;

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(IntValueObject<T>? left, int? right) => left?.Value == right;
        public static bool operator !=(IntValueObject<T>? left, int? right) => left?.Value != right;
    }

    public abstract class StringValueObject<T> : ValueObject<T>, IEquatable<string>, IComparable<T> where T : StringValueObject<T> {
        public abstract string Value { get; }

        public override string ToString() => Value;

        public int CompareTo(T? other) => Value.CompareTo(other?.Value);

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return Value.ToLower();
        }

        public override bool Equals(object? obj) => obj switch {
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
