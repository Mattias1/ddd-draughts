using Draughts.Common.OoConcepts;
using NodaTime;
using System;
using System.Collections.Generic;

namespace Draughts.Domain.GameContext.Models {
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
            6 => 6,
            _ => throw new InvalidOperationException("Unknown board size: " + BoardSize)
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

        public string Description {
            get => this == International ? "International"
                : this == EnglishAmerican ? "English draughts"
                : this == Mini ? "Mini 6x6"
                : $"Custom {BoardSize}x{BoardSize}";
        }

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return BoardSize;
            yield return FirstMove;
            yield return FlyingKings;
            yield return MenCaptureBackwards;
            yield return CaptureConstraints;
        }

        public static GameSettings International {
            get => new GameSettings(10, Color.White, true, true, DraughtsCaptureConstraints.MaximumPieces);
        }
        public static GameSettings EnglishAmerican {
            get => new GameSettings(8, Color.Black, false, false, DraughtsCaptureConstraints.AnyFinishedSequence);
        }
        public static GameSettings Mini {
            get => new GameSettings(6, Color.White, true, true, DraughtsCaptureConstraints.MaximumPieces);
        }
    }
}
