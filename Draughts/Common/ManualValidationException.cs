using System;

namespace Draughts.Common;

public sealed class ManualValidationException : Exception {
    public string Field { get; }

    public ManualValidationException(string message) : this("", message) { }
    public ManualValidationException(string field, string message) : base(message) {
        Field = field;
    }
}
