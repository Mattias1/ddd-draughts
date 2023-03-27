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

public sealed class GameSwitchTurnTest {
    private readonly IClock _fakeClock;

    public GameSwitchTurnTest() {
        _fakeClock = FakeClock.FromUtc(2030, 03, 30);
    }

    [Fact]
    public void SwitchToNextTurnWhenItsOk() {
        var game = GameTestHelper.StartedMiniGame()
            .WithTurn(Color.White)
            .Build();
        var white = game.Players.Single(p => p.Color == Color.White);

        game.NextTurn(white.UserId, _fakeClock.UtcNow());

        game.Turn.Should().NotBeNull();
        game.Turn!.Player.Color.Should().Be(Color.Black);
        game.Turn!.ExpiresAt.ToInstant().Should().BeGreaterThan(_fakeClock.GetCurrentInstant());
    }

    [Fact]
    public void DontSwitchTurnWhenItsNotYourTurn() {
        var game = GameTestHelper.StartedMiniGame()
            .WithTurn(Color.White)
            .Build();
        var black = game.Players.Single(p => p.Color == Color.Black);

        var nextTurnAction = () => game.NextTurn(black.UserId, _fakeClock.UtcNow());

        nextTurnAction.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void DontSwitchTurnWhenTheGameIsPending() {
        var player = PlayerTestHelper.Black().Build();
        var game = GameTestHelper.PendingMiniGame(player).Build();

        var nextTurnAction = () => game.NextTurn(player.UserId, _fakeClock.UtcNow());

        nextTurnAction.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void DontSwitchTurnWhenTheGameIsFinished() {
        var game = GameTestHelper.FinishedMiniGame().Build();
        var player = game.Players.First();

        var nextTurnAction = () => game.NextTurn(player.UserId, _fakeClock.UtcNow());

        nextTurnAction.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void MissTurnWhenItsExpired() {
        var game = GameTestHelper.StartedMiniGame().Build();
        var originalTurn = game.Turn ?? throw new InvalidOperationException("The turn should not be null as the game is started.");
        var otherPlayer = game.Players.Single(p => p != originalTurn.Player);

        game.MissTurn(originalTurn.ExpiresAt.PlusSeconds(3));

        game.Turn.Should().BeNull();
        game.Victor.Should().Be(otherPlayer);
        game.FinishedAt.Should().NotBeNull();
    }

    [Fact]
    public void DontMissTurnWhenItsNotExpiredYet() {
        var game = GameTestHelper.StartedMiniGame().Build();
        var originalTurn = game.Turn ?? throw new InvalidOperationException("The turn should not be null as the game is started.");

        var missTurnAction = () => game.MissTurn(originalTurn.ExpiresAt.PlusSeconds(-3));

        missTurnAction.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void DontMissTurnWhenGameIsPending() {
        var game = GameTestHelper.PendingMiniGame().Build();

        var missTurnAction = () => game.MissTurn(_fakeClock.UtcNow());

        missTurnAction.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void DontMissTurnWhenGameIsFinished() {
        var game = GameTestHelper.FinishedMiniGame().Build();

        var missTurnAction = () => game.MissTurn(_fakeClock.UtcNow());

        missTurnAction.Should().Throw<ManualValidationException>();
    }
}
