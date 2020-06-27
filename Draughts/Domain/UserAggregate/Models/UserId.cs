using Draughts.Common;
using System;

namespace Draughts.Domain.UserAggregate.Models {
    public readonly struct UserId : IComparable<UserId> {
        public long Id { get; }

        public UserId(long id) {
            if (id <= 0) {
                throw new ManualValidationException("Invalid user id.");
            }
            Id = id;
        }

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
