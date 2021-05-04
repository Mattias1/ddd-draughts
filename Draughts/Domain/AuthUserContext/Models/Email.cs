using Draughts.Common;
using Draughts.Common.OoConcepts;
using System.Text.RegularExpressions;

namespace Draughts.Domain.AuthUserContext.Models {
    public class Email : StringValueObject<Email> {
        public override string Value { get; }

        private static readonly Regex EmailRegex = new Regex(@".+@.+\..+");

        public Email(string? emailAddress) {
            if (string.IsNullOrWhiteSpace(emailAddress) || !EmailRegex.Match(emailAddress).Success) {
                throw new ManualValidationException("Invalid email address");
            }
            Value = emailAddress;
        }

        public static implicit operator string(Email email) => email.Value;
    }
}
