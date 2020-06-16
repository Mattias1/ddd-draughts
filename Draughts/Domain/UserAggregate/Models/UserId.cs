using System;

namespace Draughts.Domain.UserAggregate.Models {
    public readonly struct UserId : IComparable<UserId> {
        public long Id { get; }

        public UserId(long userId) => Id = userId;

        public static implicit operator long(UserId userId) => userId.Id;
        public static implicit operator string(UserId userId) => userId.ToString();

        public override string ToString() => Id.ToString();

        public override bool Equals(object? obj) => obj is UserId userId && Equals(userId);
        public bool Equals(UserId other) => Id.Equals(other.Id);
        public override int GetHashCode() => Id.GetHashCode();

        public int CompareTo(UserId other) => Id.CompareTo(other.Id);

        public static bool operator ==(UserId left, UserId right) => left.Equals(right);
        public static bool operator !=(UserId left, UserId right) => !left.Equals(right);
    }
}
