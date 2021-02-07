using Draughts.Common.OoConcepts;
using System.Collections.Generic;

namespace Draughts.Domain.GameAggregate.Models {
    public class Move : ValueObject<Move> {
        public Square From { get; }
        public Square To { get; }
        public bool IsCapture { get; }

        public Move(Square from, Square to, bool isCapture) {
            From = from;
            To = to;
            IsCapture = isCapture;
        }

        public override string ToString() => $"{From}{(IsCapture ? 'x' : '-')}{To}";

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return From;
            yield return To;
        }
    }
}