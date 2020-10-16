using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime.Testing;
using System;

namespace Draughts.Test.Domain.GameAggregate {
    [TestClass]
    public class JoinGameTest {
        FakeClock _fakeClock = FakeClock.FromUtc(2020, 02, 29);

        [TestMethod]
        public void JoinGame_WhenGameStarted_ThenValidationError() {
            var game = GameTestHelper.StartedInternationalGame().Build();

            var thirdPlayer = PlayerTestHelper.White()
                .WithId(IdTestHelper.Next())
                .WithUsername("Ender")
                .Build();

            Action joinGame = () => game.JoinGame(thirdPlayer, _fakeClock.UtcNow());

            joinGame.Should().Throw<ManualValidationException>();
        }

        [TestMethod]
        public void JoinGame_WhenAlreadyInGame_ThenValidationError() {
            var whitePlayer = PlayerTestHelper.White().Build();
            var game = GameTestHelper.PendingInternationalGame(whitePlayer).Build();

            Action joinGame = () => game.JoinGame(whitePlayer, _fakeClock.UtcNow());

            joinGame.Should().Throw<ManualValidationException>();
        }

        [TestMethod]
        public void JoinGame_WhenUserIdOccupied_ThenValidationError() {
            var whitePlayer = PlayerTestHelper.White().WithUserId(9999).Build();
            var game = GameTestHelper.PendingInternationalGame(whitePlayer).Build();

            var blackPlayer = PlayerTestHelper.Black().WithUserId(9999).Build();

            Action joinGame = () => game.JoinGame(blackPlayer, _fakeClock.UtcNow());

            joinGame.Should().Throw<ManualValidationException>();
        }

        [TestMethod]
        public void JoinGame_WhenColorOccupied_ThenValidationError() {
            var whitePlayer = PlayerTestHelper.White().Build();
            var game = GameTestHelper.PendingInternationalGame(whitePlayer).Build();

            var otherWhitePlayer = PlayerTestHelper.White().Build();

            Action joinGame = () => game.JoinGame(whitePlayer, _fakeClock.UtcNow());

            joinGame.Should().Throw<ManualValidationException>();
        }

        [TestMethod]
        public void JoinGame_WhenFirstPlayer_ThenJoinGame() {
            var game = GameTestHelper.PendingInternationalGame().Build();
            var whitePlayer = PlayerTestHelper.White().Build();

            game.JoinGame(whitePlayer, _fakeClock.UtcNow());

            game.Players.Should().OnlyContain(p => p == whitePlayer);
            game.HasStarted.Should().BeFalse();
            game.Turn.Should().BeNull();
        }

        [TestMethod]
        public void JoinGame_WhenSecondPlayer_ThenStartGame() {
            var blackPlayer = PlayerTestHelper.Black().Build();
            var game = GameTestHelper.PendingInternationalGame(blackPlayer).Build();

            var whitePlayer = PlayerTestHelper.White().Build();

            game.JoinGame(whitePlayer, _fakeClock.UtcNow());

            game.Players.Should().BeEquivalentTo(whitePlayer, blackPlayer);
            game.HasStarted.Should().BeTrue();
            game.Turn.Should().NotBeNull();
            game.Turn!.Player.Color.Should().Be(Color.White);
        }
    }
}
