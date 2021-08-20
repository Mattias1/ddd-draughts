using Draughts.Common;
using Draughts.Common.OoConcepts;
using System;
using System.Text.RegularExpressions;

namespace Draughts.Domain.AuthUserContext.Models {
    public class Username : StringValueObject<Username> {
        public const string ADMIN = "admin";
        public const string MATTY = "Matty";
        public const int MAX_LENGTH = 25;

        public override string Value { get; }

        public Username(string? value) {
            ValidateNameOrThrow(value);
            Value = value ?? throw new InvalidOperationException("Name cannot be null.");
        }

        private static void ValidateNameOrThrow(string? name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ManualValidationException("The name cannot be empty.");
            }
            if (name.Length > MAX_LENGTH) {
                throw new ManualValidationException($"The name can be at most {MAX_LENGTH} characters.");
            }
            var regex = new Regex("[^a-zA-Z-0-9-_]");
            if (regex.IsMatch(name)) {
                throw new ManualValidationException("The name can only contain normal letters, numbers, "
                    + "a dash or an underscore.");
            }
        }

        public static implicit operator string(Username? username) => username?.Value ?? "";
    }
}
