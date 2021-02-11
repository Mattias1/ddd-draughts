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

    public class PossibleMove : Move {
        public bool MoreCapturesAvailable;
        public Square? Victim { get; }

        public PossibleMove(Square from, Square to, Square? victim = null, bool moreCapturesAvailable = false)
                : base(from, to, victim != null) {
            MoreCapturesAvailable = moreCapturesAvailable;
            Victim = victim;
        }
    }
}