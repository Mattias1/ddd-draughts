using Draughts.Domain.GameContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Draughts.Domain.GameContext.Models.GameSettings;

namespace Draughts.Test.Domain.GameContext;

public sealed class PossibleMoveCalculatorTest {
    // TODO: You can't move over captured pieces!!!
    // AKA: Pieces are only removed after the capture is completed.
    [Fact]
    public void ManMoves() {
        // |_|4|_|.|
        // |.|_|.|_|
        // |_|5|_|.|
        // |.|_|5|_|
        CalculatePossibleMoves("40005005", Color.White, InternationalSettings(4))
            .Should().BeEquivalentTo("5-3", "5-4", "8-6");
    }

    [Fact]
    public void ManCaptures() {
        // |_|.|_|4|
        // |.|_|5|_|
        // |_|.|_|4|
        // |.|_|5|_|
        CalculatePossibleMoves("04050405", Color.Black, InternationalSettings(4))
            .Should().BeEquivalentTo("2x5", "6x1");
    }

    [Fact]
    public void NoMovesWhenCapturing() {
        // |_|4|_|.|
        // |.|_|4|_|
        // |_|5|_|4|
        // |.|_|.|_|
        CalculatePossibleMoves("40045400", Color.Black, InternationalSettings(4))
            .Should().BeEquivalentTo("4x7");
    }

    [Fact]
    public void KingMoves() {
        // |_|7|_|.|_|4|
        // |.|_|.|_|.|_|
        // |_|.|_|6|_|.|
        // |6|_|.|_|5|_|
        // |_|4|_|.|_|4|
        // |6|_|.|_|5|_|
        CalculatePossibleMoves("704 000 060 605 404 605", Color.Black, InternationalSettings(6))
            .Should().BeEquivalentTo("3-6", "8-5", "8-6", "8-11", "10-7", "10-5", "10-2", "13-17");
    }

    [Fact]
    public void KingCaptures() {
        // |_|.|_|.|_|.|
        // |.|_|5|_|.|_|
        // |_|6|_|.|_|4|
        // |.|_|4|_|7|_|
        // |_|.|_|4|_|.|
        // |7|_|.|_|.|_|
        CalculatePossibleMoves("000 050 604 047 040 700", Color.White, InternationalSettings(6))
            .Should().BeEquivalentTo("5x10", "12x17", "16x8", "16x6", "16x3");
    }

    [Fact]
    public void KingCapturesOnePerJump() {
        // |_|.|_|.|_|.|
        // |.|_|.|_|.|_|
        // |_|.|_|4|_|.|
        // |.|_|.|_|.|_|
        // |_|4|_|.|_|.|
        // |7|_|.|_|.|_|
        CalculatePossibleMoves("000 000 040 000 400 700", Color.White, InternationalSettings(6))
            .Should().BeEquivalentTo("16x11");
    }

    [Fact]
    public void ChainCaptures() {
        // |_|.|_|.|_|.|
        // |.|_|4|_|.|_|
        // |_|.|_|.|_|.|
        // |.|_|4|_|4|_|
        // |_|.|_|5|_|.|
        // |.|_|.|_|.|_|
        CalculatePossibleMoves("000 040 000 044 050 000", Color.White, InternationalSettings(6))
            .Should().BeEquivalentTo("14x7");
    }

    [Fact]
    public void LongestChainCapturesMan() {
        // |_|.|_|.|_|.|
        // |5|_|.|_|.|_|
        // |_|4|_|4|_|.|
        // |.|_|.|_|.|_|
        // |_|4|_|4|_|.|
        // |.|_|5|_|.|_|
        CalculatePossibleMoves("000 500 440 000 440 050", Color.White, InternationalSettings(6))
            .Should().BeEquivalentTo("17x10", "17x12");
    }

    [Fact]
    public void LongestChainCapturesKing() {
        // |_|.|_|.|_|7|
        // |.|_|4|_|4|_|
        // |_|.|_|.|_|.|
        // |.|_|.|_|.|_|
        // |_|4|_|4|_|.|
        // |.|_|7|_|.|_|
        CalculatePossibleMoves("007 044 000 000 440 070", Color.White, InternationalSettings(6))
            .Should().BeEquivalentTo("17x10", "17x9");
    }

    [Fact]
    public void BritishMenCannotCaptureBackwards() {
        // |_|.|_|.|
        // |.|_|5|_|
        // |_|4|_|.|
        // |.|_|.|_|
        CalculatePossibleMoves("00054000", Color.White, BritishSettings(4))
            .Should().BeEquivalentTo("4-1", "4-2");
    }

    [Fact]
    public void BritishKingsCanMoveBackwards() {
        // |_|.|_|.|
        // |.|_|7|_|
        // |_|.|_|.|
        // |6|_|.|_|
        CalculatePossibleMoves("00070060", Color.White, BritishSettings(4))
            .Should().BeEquivalentTo("4-1", "4-2", "4-5", "4-6");
    }

    [Fact]
    public void BritishKingsCanCaptureBackwards() {
        // |_|.|_|.|
        // |.|_|7|_|
        // |_|6|_|.|
        // |.|_|.|_|
        CalculatePossibleMoves("00076000", Color.White, BritishSettings(4))
            .Should().BeEquivalentTo("4x7");
    }

    [Fact]
    public void BritishKingsCannotFly() {
        // |_|.|_|7|
        // |.|_|.|_|
        // |_|4|_|.|
        // |.|_|.|_|
        CalculatePossibleMoves("07004000", Color.White, BritishSettings(4))
            .Should().BeEquivalentTo("2-4");
    }

    [Fact]
    public void BritishKingsReallyCannotFly() {
        // |_|.|_|7|
        // |.|_|4|_|
        // |_|.|_|.|
        // |.|_|.|_|
        CalculatePossibleMoves("07040000", Color.White, BritishSettings(4))
            .Should().BeEquivalentTo("2x5"); // No 2x7 in here.
    }

    [Fact]
    public void AnyChainCaptureForBritishMen() {
        // |_|.|_|.|_|5|
        // |.|_|4|_|4|_|
        // |_|.|_|.|_|.|
        // |.|_|4|_|4|_|
        // |_|4|_|5|_|5|
        // |.|_|5|_|.|_|
        CalculatePossibleMoves("005 044 000 044 455 050", Color.White, BritishSettings(6))
            .Should().BeEquivalentTo("14x7", "14x9", "15x8");
    }

    [Fact]
    public void AnyChainCaptureForBritishKings() {
        // |_|.|_|.|_|7|
        // |.|_|4|_|4|_|
        // |_|.|_|.|_|.|
        // |.|_|4|_|4|_|
        // |_|6|_|7|_|.|
        // |.|_|7|_|.|_|
        CalculatePossibleMoves("007 044 000 044 670 070", Color.White, BritishSettings(6))
            .Should().BeEquivalentTo("3x8", "14x7", "14x9");
    }

    [Fact]
    public void AnyChainCaptureForBritishCustomizedWithFlyingKings() {
        // |_|.|_|.|_|7|
        // |.|_|4|_|4|_|
        // |_|.|_|.|_|.|
        // |.|_|.|_|.|_|
        // |_|4|_|4|_|.|
        // |.|_|7|_|.|_|
        var settings = new GameSettings(6, Color.Black, true, false, DraughtsCaptureConstraints.AnyFinishedSequence);
        CalculatePossibleMoves("007 044 000 000 440 070", Color.White, settings)
            .Should().BeEquivalentTo("3x8", "3x11", "17x10", "17x12", "17x9");
    }

    [Fact]
    public void BritishMenCannotCaptureBackwardsInChains() {
        // |_|.|_|.|_|.|
        // |.|_|.|_|4|_|
        // |_|.|_|.|_|5|
        // |.|_|.|_|.|_|
        // |_|4|_|4|_|.|
        // |.|_|.|_|5|_|
        CalculatePossibleMoves("000 004 005 000 440 005", Color.White, BritishSettings(6))
            .Should().BeEquivalentTo("9x2", "18x11");
    }

    [Fact]
    public void BritishKingsCanCaptureBackwardsInChains() {
        // |_|.|_|.|_|.|
        // |.|_|.|_|4|_|
        // |_|.|_|.|_|7|
        // |.|_|.|_|.|_|
        // |_|4|_|4|_|.|
        // |.|_|.|_|7|_|
        CalculatePossibleMoves("000 004 007 000 440 007", Color.White, BritishSettings(6))
            .Should().BeEquivalentTo("18x11");
    }

    [Fact]
    public void OnlyCaptureFromRestrictedFieldWhenChaining() {
        // |_|.|_|.|
        // |.|_|5|_|
        // |_|4|_|4|
        // |.|_|5|_|
        var board = Board.FromString("00054405");
        var posisbleMoves = PossibleMoveCalculator.ForChainCaptures(board, 5.AsSquare(), InternationalSettings(4))
            .Calculate()
            .Select(m => m.ToString())
            .ToList();
        posisbleMoves.Should().BeEquivalentTo("5x2");
    }

    [Fact]
    public void CannotCaptureDeadPieces() {
        // |_|.|_|.|_|.|
        // |.|_|5|_|D|_|
        // |_|.|_|4|_|.|
        // |.|_|7|_|F|_|
        // |_|.|_|.|_|.|
        // |.|_|.|_|.|_|
        var board = Board.FromString("000 05D 040 07F 000 000");
        var posisbleMoves = PossibleMoveCalculator.ForChainCaptures(board, 8.AsSquare(), InternationalSettings(6))
            .Calculate()
            .Select(m => m.ToString())
            .ToList();
        posisbleMoves.Should().BeEquivalentTo("8x1", "8x13");
    }

    [Fact]
    public void DontCapturePiecesTwiceInAChainCapture() {
        // |_|5|_|.|_|.|_|.|_|.|
        // |.|_|5|_|.|_|4|_|4|_|
        // |_|.|_|5|_|7|_|.|_|.|
        // |.|_|.|_|5|_|4|_|4|_|
        // |_|.|_|.|_|5|_|.|_|.|
        // |.|_|.|_|.|_|5|_|4|_|
        // |_|.|_|.|_|.|_|5|_|.|
        // |.|_|4|_|.|_|.|_|.|_|
        // |_|.|_|.|_|4|_|4|_|5|
        // |.|_|.|_|.|_|.|_|.|_|
        string boardString = "50000 05044 05700 00544 00500 00054 00050 04000 00445 00000";
        CalculatePossibleMoves(boardString, Color.White, InternationalSettings(10))
            .Should().BeEquivalentTo("13x4");
    }

    private static List<string> CalculatePossibleMoves(string boardString, Color color, GameSettings settings) {
        var board = Board.FromString(boardString);
        return PossibleMoveCalculator.ForNewTurn(board, color, settings)
            .Calculate()
            .Select(m => m.ToString())
            .ToList();
    }

    private GameSettings InternationalSettings(int boardSize) {
        return new GameSettings(boardSize, Color.White, true, true, DraughtsCaptureConstraints.MaximumPieces);
    }

    private GameSettings BritishSettings(int boardSize) {
        return new GameSettings(boardSize, Color.Black, false, false, DraughtsCaptureConstraints.AnyFinishedSequence);
    }
}
