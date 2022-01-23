using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.GameContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime.Testing;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameContext {
    public class JoinGameTest {
        FakeClock _fakeClock = FakeClock.FromUtc(2020, 02, 29);

        [Fact]
        public void JoiningWhenStartedThrowsValidationError() {
            var game = GameTestHelper.StartedInternationalGame().Build();

            var thirdPlayer = PlayerTestHelper.White()
                .WithId(IdTestHelper.Next())
                .WithUsername("Ender")
                .Build();

            Action joinGame = () => game.JoinGame(thirdPlayer, _fakeClock.UtcNow());

            joinGame.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void JoiningWhenAlreadyInGameThrowsValidationError() {
            var whitePlayer = PlayerTestHelper.White().Build();
            var game = GameTestHelper.PendingInternationalGame(whitePlayer).Build();

            Action joinGame = () => game.JoinGame(whitePlayer, _fakeClock.UtcNow());

            joinGame.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void JoiningWhenUserIdOccupiedThrowsValidationError() {
            var whitePlayer = PlayerTestHelper.White().WithUserId(9999).Build();
            var game = GameTestHelper.PendingInternationalGame(whitePlayer).Build();

            var blackPlayer = PlayerTestHelper.Black().WithUserId(9999).Build();

            Action joinGame = () => game.JoinGame(blackPlayer, _fakeClock.UtcNow());

            joinGame.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void JoiningWhenColorOccupiedThrowsValidationError() {
            var whitePlayer = PlayerTestHelper.White().Build();
            var game = GameTestHelper.PendingInternationalGame(whitePlayer).Build();

            var otherWhitePlayer = PlayerTestHelper.White().Build();

            Action joinGame = () => game.JoinGame(whitePlayer, _fakeClock.UtcNow());

            joinGame.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void JoiningAsFirstPlayerJustJoins() {
            var game = GameTestHelper.PendingInternationalGame().Build();
            var whitePlayer = PlayerTestHelper.White().Build();

            game.JoinGame(whitePlayer, _fakeClock.UtcNow());

            game.Players.Should().OnlyContain(p => p == whitePlayer);
            game.HasStarted.Should().BeFalse();
            game.Turn.Should().BeNull();
        }

        [Fact]
        public void JoiningAsSecondPlayerStartsGame() {
            var blackPlayer = PlayerTestHelper.Black().Build();
            var game = GameTestHelper.PendingInternationalGame(blackPlayer).Build();

            var whitePlayer = PlayerTestHelper.White().Build();

            game.JoinGame(whitePlayer, _fakeClock.UtcNow());

            game.Players.Should().BeEquivalentTo(new[] { whitePlayer, blackPlayer });
            game.HasStarted.Should().BeTrue();
            game.Turn.Should().NotBeNull();
            game.Turn!.Player.Color.Should().Be(Color.White);
        }
    }
}
