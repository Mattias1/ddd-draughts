using Draughts.Common;
using System;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public readonly struct AuthUserId : IComparable<AuthUserId> {
        public long Id { get; }

        public AuthUserId(long id) {
            if (id <= 0) {
                throw new ManualValidationException("Invalid user id.");
            }
            Id = id;
        }

        public static implicit operator long(AuthUserId authUserId) => authUserId.Id;
        public static implicit operator string(AuthUserId authUserId) => authUserId.ToString();

        public override string ToString() => Id.ToString();

        public override bool Equals(object? obj) => obj is AuthUserId authUserId && Equals(authUserId);
        public bool Equals(AuthUserId other) => Id.Equals(other.Id);
        public override int GetHashCode() => Id.GetHashCode();

        public int CompareTo(AuthUserId other) => Id.CompareTo(other.Id);

        public static bool operator ==(AuthUserId left, AuthUserId right) => left.Equals(right);
        public static bool operator !=(AuthUserId left, AuthUserId right) => !left.Equals(right);
    }
}
