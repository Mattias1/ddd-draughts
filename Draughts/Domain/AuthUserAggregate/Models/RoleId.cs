using Draughts.Common;
using System;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public readonly struct RoleId : IComparable<RoleId> {
        public long Id { get; }

        public RoleId(long id) {
            if (id <= 0) {
                throw new ManualValidationException("Invalid user id.");
            }
            Id = id;
        }


        public static implicit operator long(RoleId roleId) => roleId.Id;
        public static implicit operator string(RoleId roleId) => roleId.ToString();

        public override string ToString() => Id.ToString();

        public override bool Equals(object? obj) => obj is RoleId roleId && Equals(roleId);
        public bool Equals(RoleId other) => Id.Equals(other.Id);
        public override int GetHashCode() => Id.GetHashCode();

        public int CompareTo(RoleId other) => Id.CompareTo(other.Id);

        public static bool operator ==(RoleId left, RoleId right) => left.Equals(right);
        public static bool operator !=(RoleId left, RoleId right) => !left.Equals(right);
    }
}
