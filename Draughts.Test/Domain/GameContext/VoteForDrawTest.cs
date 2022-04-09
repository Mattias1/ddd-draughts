using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.GameContext.Services;
using Draughts.Domain.UserContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime;
using NodaTime.Testing;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public sealed class VoteForDrawTest {
    private readonly IClock _fakeClock;
    private readonly PlayGameDomainService _playGameDomainService;

    public VoteForDrawTest() {
        _fakeClock = FakeClock.FromUtc(2020, 01, 16, 12, 0, 0);
        _playGameDomainService = new PlayGameDomainService(_fakeClock);
    }

    [Fact]
    public void CanAddFirstVoteForDraw() {
        var game = GameTestHelper.StartedMiniGame().Build();
        var voting = VotingTestHelper.Draw().Build();

        _playGameDomainService.VoteForDraw(game, voting, game.Players[0].UserId);

        voting.Votes.Count.Should().Be(1);
    }

    [Fact]
    public void DrawGameWithSecondVote() {
        var game = GameTestHelper.StartedMiniGame().Build();
        var voting = VotingTestHelper.Draw()
            .AddVoteForDraw(game.Players[0].UserId, _fakeClock.UtcNow())
            .Build();

        _playGameDomainService.VoteForDraw(game, voting, game.Players[1].UserId);

        game.FinishedAt.Should().Be(_fakeClock.UtcNow());
        game.Victor.Should().BeNull();
    }

    [Fact]
    public void DontVoteForFinishedGames() {
        var game = GameTestHelper.FinishedMiniGame(Color.White).Build();
        var voting = VotingTestHelper.Draw().Build();

        Action voteFunc = () => _playGameDomainService.VoteForDraw(game, voting, game.Players[0].UserId);

        voteFunc.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void DontVoteForOtherPplsGames() {
        var game = GameTestHelper.StartedMiniGame().Build();
        var voting = VotingTestHelper.Draw().Build();

        Action voteFunc = () => _playGameDomainService.VoteForDraw(game, voting, new UserId(999));

        voteFunc.Should().Throw<ManualValidationException>();
    }
}
