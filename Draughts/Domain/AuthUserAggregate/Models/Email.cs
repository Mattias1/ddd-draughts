using Draughts.Common;
using System;
using System.Text.RegularExpressions;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public readonly struct Email : IComparable<Email> {
        public string EmailAddress { get; }

        private static readonly Regex EmailRegex = new Regex(@".+@.+\..+");

        public Email(string emailAddress) {
            if (!EmailRegex.Match(emailAddress).Success) {
                throw new ManualValidationException("Invalid email address");
            }

            EmailAddress = emailAddress;
        }

        public static implicit operator string(Email email) => email.EmailAddress;

        public override string ToString() => EmailAddress;

        public override bool Equals(object? obj) => obj is Email email && Equals(email);
        public bool Equals(Email other) => Equals(other.EmailAddress);
        public bool Equals(string? emailAddress) => EmailAddress.Equals(emailAddress, StringComparison.OrdinalIgnoreCase);
        public override int GetHashCode() => EmailAddress.GetHashCode();

        public int CompareTo(Email other) => EmailAddress.CompareTo(other.EmailAddress);

        public static bool operator ==(Email left, Email right) => left.Equals(right);
        public static bool operator !=(Email left, Email right) => !left.Equals(right);
        public static bool operator ==(Email left, string? right) => left.Equals(right);
        public static bool operator !=(Email left, string? right) => !left.Equals(right);
    }
}
