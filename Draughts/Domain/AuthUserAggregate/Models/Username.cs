using Draughts.Common;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class Username : StringValueObject<Username> {
        public override string Value { get; }

        public Username(string? value) {
            if (string.IsNullOrWhiteSpace(value)) {
                throw new ManualValidationException("Invalid username.");
            }
            Value = value;
        }

        public static implicit operator string(Username username) => username.Value;
    }
}
