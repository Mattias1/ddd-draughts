using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.GameContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime;
using NodaTime.Testing;
using System;
using System.Linq;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public class EndGameMovesTest {
    private readonly IClock _fakeClock;

    public EndGameMovesTest() {
        _fakeClock = FakeClock.FromUtc(2020, 01, 16, 12, 0, 0);
    }

    [Fact]
    public void WinAGame() {
        var game = GameTestHelper.StartedMiniGame().Build();
        var whitePlayer = game.Players.Single(p => p.Color == Color.White);

        game.WinGame(whitePlayer.UserId, _fakeClock.UtcNow());

        game.FinishedAt.Should().Be(_fakeClock.UtcNow());
        game.Victor.Should().Be(whitePlayer);
        game.Turn.Should().BeNull();
    }

    [Fact]
    public void DontWinAGameWhenItsNotYourTurn() {
        var game = GameTestHelper.StartedMiniGame().Build();
        var blackPlayer = game.Players.Single(p => p.Color == Color.Black);

        Action winFunc = () => game.WinGame(blackPlayer.UserId, _fakeClock.UtcNow());
        winFunc.Should().Throw<ManualValidationException>();

        game.FinishedAt.Should().BeNull();
        game.Victor.Should().BeNull();
        game.Turn.Should().NotBeNull();
    }

    [Fact]
    public void EndAGameInADraw() {
        var game = GameTestHelper.StartedMiniGame().Build();

        game.Draw(_fakeClock.UtcNow());

        game.FinishedAt.Should().Be(_fakeClock.UtcNow());
        game.Victor.Should().BeNull();
        game.Turn.Should().BeNull();
    }

    [Fact]
    public void ResignAGame() {
        var game = GameTestHelper.StartedMiniGame().Build();
        var blackPlayer = game.Players.Single(p => p.Color == Color.Black);
        var whitePlayer = game.Players.Single(p => p.Color == Color.White);

        game.ResignGame(blackPlayer.UserId, _fakeClock.UtcNow());

        game.FinishedAt.Should().Be(_fakeClock.UtcNow());
        game.Victor.Should().Be(whitePlayer);
        game.Turn.Should().BeNull();
    }
}
