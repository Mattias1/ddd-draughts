using Draughts.Common.OoConcepts;
using NodaTime;
using System;
using System.Collections.Generic;

namespace Draughts.Domain.GameAggregate.Models {
    public class GameSettings : ValueObject<GameSettings> {
        public enum DraughtsCaptureConstraints { MaximumPieces, AnyFinishedSequence };

        public int BoardSize { get; }
        public Color FirstMove { get; }
        public bool FlyingKings { get; }
        public bool MenCaptureBackwards { get; }
        public DraughtsCaptureConstraints CaptureConstraints { get; }

        public Duration MaxTurnLength => Duration.FromHours(24);

        public int PiecesPerSide => BoardSize switch
        {
            12 => 30,
            10 => 20,
            8 => 12,
            _ => throw new InvalidOperationException("Unknown boardsize: " + BoardSize)
        };

        public GameSettings(
            int boardSize, Color firstMove,
            bool flyingKings, bool menCaptureBackwards, DraughtsCaptureConstraints captureConstraints
        ) {
            BoardSize = boardSize;
            FirstMove = firstMove;
            FlyingKings = flyingKings;
            MenCaptureBackwards = menCaptureBackwards;
            CaptureConstraints = captureConstraints;
        }

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return BoardSize;
            yield return FirstMove;
            yield return FlyingKings;
            yield return MenCaptureBackwards;
            yield return CaptureConstraints;
        }
    }
}
