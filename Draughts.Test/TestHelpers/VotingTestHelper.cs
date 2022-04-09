using Draughts.Common.Utilities;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using NodaTime.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.GameContext.Models.Voting;

namespace Draughts.Test.TestHelpers;

public sealed class VotingTestHelper {
    private static readonly ZonedDateTime Feb29 = FakeClock.FromUtc(2020, 02, 29).UtcNow();

    public static VotingBuilder Draw() {
        return new VotingBuilder()
            .WithId(new GameId(IdTestHelper.NextForGame()));
    }


    public sealed class VotingBuilder {
        private GameId? _id;
        private List<Vote> _votes = new List<Vote>();

        public VotingBuilder WithId(long id) => WithId(new GameId(id));
        public VotingBuilder WithId(GameId id) {
            _id = id;
            return this;
        }

        public VotingBuilder WithVotes(params Vote[] votes) => WithVotes(votes.ToList());
        public VotingBuilder WithVotes(List<Vote> votes) {
            _votes = votes;
            return this;
        }

        public VotingBuilder AddVoteForDraw(long userId, ZonedDateTime createdAd)
            => AddVoteForDraw(new UserId(userId), createdAd);
        public VotingBuilder AddVoteForDraw(UserId userId, ZonedDateTime createdAd)
            => AddVoteInFavor(userId, VotingSubject.Draw, createdAd);
        public VotingBuilder AddVoteInFavor(UserId userId, VotingSubject subject, ZonedDateTime createdAt) {
            _votes.Add(new Vote(userId, subject, true, createdAt));
            return this;
        }

        public Voting Build() {
            if (_id is null) {
                throw new InvalidOperationException("Id is not nullable");
            }

            return new Voting(_id, _votes);
        }
    }
}
