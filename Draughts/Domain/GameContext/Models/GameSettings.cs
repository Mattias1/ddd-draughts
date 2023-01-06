using Draughts.Common.OoConcepts;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Domain.GameContext.Models;

public sealed class GameSettings : ValueObject<GameSettings> {
    public enum GameSettingsPreset { International, EnglishAmerican, Mini, Hexdame, MiniHex, Other };
    public enum DraughtsCaptureConstraints { MaximumPieces, AnyFinishedSequence };

    public int BoardSize { get; }
    public Color FirstMove { get; }
    public bool FlyingKings { get; }
    public bool MenCaptureBackwards { get; }
    public DraughtsCaptureConstraints CaptureConstraints { get; }
    public Duration MaxTurnLength { get; }
    public IBoardType BoardType { get; }

    public GameSettings(int boardSize, Color firstMove, bool flyingKings, bool menCaptureBackwards,
            DraughtsCaptureConstraints captureConstraints)
            : this(boardSize, firstMove, flyingKings, menCaptureBackwards,
            captureConstraints, Duration.FromHours(24)) { }

    public GameSettings(int boardSize, Color firstMove, bool flyingKings, bool menCaptureBackwards,
            DraughtsCaptureConstraints captureConstraints, Duration turnLength) {
        BoardSize = boardSize;
        FirstMove = firstMove;
        FlyingKings = flyingKings;
        MenCaptureBackwards = menCaptureBackwards;
        CaptureConstraints = captureConstraints;
        MaxTurnLength = turnLength;
        BoardType = IsHexagonalBoard() ? new HexagonalBoardType(boardSize) : new SquareBoardType(boardSize);
    }

    public string Description {
        get => this == International ? "International"
            : this == EnglishAmerican ? "English draughts"
            : this == Mini ? "Mini 6x6"
            : this == Hexdame ? "Hexdame"
            : this == MiniHex ? "MiniHex"
            : IsHexagonalBoard() ? $"Custom hex-{BoardSize}"
            : $"Custom {BoardSize}x{BoardSize}";
    }

    private bool IsHexagonalBoard() => BoardSize == 3 || BoardSize == 5;

    public GameSettingsPreset PresetEnum {
        get => this == International ? GameSettingsPreset.International
            : this == EnglishAmerican ? GameSettingsPreset.EnglishAmerican
            : this == Mini ? GameSettingsPreset.Mini
            : this == Hexdame ? GameSettingsPreset.Hexdame
            : this == MiniHex ? GameSettingsPreset.MiniHex
            : GameSettingsPreset.Other;
    }

    protected override IEnumerable<object> GetEqualityComponents() {
        yield return BoardSize;
        yield return FirstMove;
        yield return FlyingKings;
        yield return MenCaptureBackwards;
        yield return CaptureConstraints;
    }

    public GameSettings WithTurnTime(Duration newDuration) {
        return new GameSettings(BoardSize, FirstMove, FlyingKings, MenCaptureBackwards, CaptureConstraints, newDuration);
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
    public static GameSettings Hexdame {
        get => new GameSettings(5, Color.White, true, true, DraughtsCaptureConstraints.MaximumPieces);
    }
    public static GameSettings MiniHex {
        get => new GameSettings(3, Color.White, true, true, DraughtsCaptureConstraints.MaximumPieces);
    }
}
