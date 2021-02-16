using Draughts.Common.OoConcepts;
using System.Collections.Generic;

namespace Draughts.Domain.GameAggregate.Models {
    public class Move : ValueObject<Move> {
        public SquareId From { get; }
        public SquareId To { get; }
        public bool IsCapture { get; }

        protected Move(SquareId from, SquareId to, bool isCapture) {
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
        public SquareId? Victim { get; }

        private PossibleMove(SquareId from, SquareId to, SquareId? victim, bool moreCapturesAvailable)
                : base(from, to, victim is not null) {
            MoreCapturesAvailable = moreCapturesAvailable;
            Victim = victim;
        }

        public static PossibleMove NormalMove(Square from, Square to) => NormalMove(from.Id, to.Id);
        public static PossibleMove NormalMove(SquareId from, SquareId to) => new PossibleMove(from, to, null, false);

        public static PossibleMove CaptureMove(Square from, Square to, Square? victim, bool moreCapturesAvailable) {
            return CaptureMove(from.Id, to.Id, victim?.Id, moreCapturesAvailable);
        }
        public static PossibleMove CaptureMove(SquareId from, SquareId to, SquareId? victim, bool moreCapturesAvailable) {
            return new PossibleMove(from, to, victim, moreCapturesAvailable);
        }
    }
}
