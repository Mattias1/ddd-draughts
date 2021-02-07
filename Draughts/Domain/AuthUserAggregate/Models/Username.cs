using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class Username : StringValueObject<Username> {
        public const int MAX_LENGTH = 50;

        public override string Value { get; }

        public Username(string? value) {
            if (string.IsNullOrWhiteSpace(value) || value.Length > MAX_LENGTH) {
                throw new ManualValidationException("Invalid username.");
            }
            Value = value;
        }

        public static implicit operator string(Username username) => username.Value;
    }
}
