using System;

namespace Draughts.Common {
    public class ManualValidationException : Exception {
        public ManualValidationException(string message) : base(message) { }
    }
}
