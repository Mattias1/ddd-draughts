using Draughts.Common;
using System;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public struct Username : IComparable<Username> {
        public string Value { get; }

        public Username(string? value) {
            if (string.IsNullOrWhiteSpace(value)) {
                throw new ManualValidationException("Invalid username.");
            }
            Value = value;
        }

        public static implicit operator string(Username username) => username.Value;

        public override string ToString() => Value;

        public override bool Equals(object? obj) => obj is Username username && Equals(username);
        public bool Equals(Username other) => Equals(other.Value);
        public bool Equals(string? value) => Value.Equals(value, StringComparison.OrdinalIgnoreCase);
        public override int GetHashCode() => Value.GetHashCode();

        public int CompareTo(Username other) => Value.CompareTo(other.Value);

        public static bool operator ==(Username left, Username right) => left.Equals(right);
        public static bool operator !=(Username left, Username right) => !left.Equals(right);
        public static bool operator ==(Username left, string? right) => left.Equals(right);
        public static bool operator !=(Username left, string? right) => !left.Equals(right);
    }
}
