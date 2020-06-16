using System;

namespace Draughts.Domain.UserAggregate.Models {
    public readonly struct Rating : IComparable<Rating> {
        public static Rating Default = new Rating(1000);

        public int Value { get; }

        public Rating(int value) => Value = value;

        public static implicit operator int(Rating rating) => rating.Value;

        public override string ToString() => Value.ToString();

        public override bool Equals(object? obj) => obj is Rating rating && Equals(rating);
        public bool Equals(Rating other) => Value.Equals(other.Value);
        public override int GetHashCode() => Value.GetHashCode();

        public int CompareTo(Rating other) => Value.CompareTo(other.Value);

        public static bool operator ==(Rating left, Rating right) => left.Equals(right);
        public static bool operator !=(Rating left, Rating right) => !left.Equals(right);
    }
}
