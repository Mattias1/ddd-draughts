using Draughts.Common;
using Draughts.Common.OoConcepts;
using System.Text.RegularExpressions;

namespace Draughts.Domain.AuthUserContext.Models {
    public class Email : StringValueObject<Email> {
        public const int MAX_LENGTH = 200;

        public override string Value { get; }

        private static readonly Regex EmailRegex = new Regex(@".+@.+\..+");

        public Email(string? emailAddress) {
            if (string.IsNullOrWhiteSpace(emailAddress)
                    || emailAddress.Length > MAX_LENGTH
                    || !EmailRegex.Match(emailAddress).Success) {
                throw new ManualValidationException("Invalid email address");
            }
            Value = emailAddress;
        }

        public static implicit operator string(Email? email) => email?.Value ?? "";
    }
}
