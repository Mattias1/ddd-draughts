using Draughts.Common.Utilities;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime.Testing;

namespace Draughts.Test.Domain.GameAggregate {
    [TestClass]
    public class PlayGameTest {
        FakeClock _fakeClock = FakeClock.FromUtc(2020, 02, 29);

        [TestMethod]
        public void DoMoveShouldUpdateBoardAndSwitchTurn() {
            var game = GameTestHelper.StartedInternationalGame().Build();
            var from = SquareNumber.FromPosition(3, 6, game.Settings.BoardSize);
            var to = SquareNumber.FromPosition(4, 5, game.Settings.BoardSize);

            var result = game.DoMove(from, to, _fakeClock.UtcNow());

            result.Should().Be(GameState.MoveResult.NextTurn);
            game.GameState.Board[3, 6].Should().Be(Piece.Empty);
            game.GameState.Board[4, 5].Should().Be(Piece.WhiteMan);
            game.Turn!.Player.Color.Should().Be(Color.Black);
        }
    }
}
