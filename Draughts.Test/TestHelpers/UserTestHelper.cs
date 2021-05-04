using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using NodaTime.Testing;
using System;

namespace Draughts.Test.TestHelpers {
    public class UserTestHelper {
        private static readonly ZonedDateTime Feb29 = FakeClock.FromUtc(2020, 02, 29).UtcNow();

        public static UserBuilder User(string name = "user") {
            return new UserBuilder()
                .WithId(IdTestHelper.NextForUser())
                .WithUsername(name)
                .WithRating(Rating.StartRating)
                .WithRank(Rank.Ranks.Private)
                .WithGamesPlayed(0)
                .WithCreatedAt(Feb29);
        }


        public class UserBuilder {
            private UserId? _id;
            private Username? _username;
            private Rating? _rating;
            private Rank? _rank;
            private int _gamesPlayed;
            private ZonedDateTime? _createdAt;

            public UserBuilder WithId(long id) => WithId(new UserId(id));
            public UserBuilder WithId(UserId id) {
                _id = id;
                return this;
            }

            public UserBuilder WithUsername(string username) => WithUsername(new Username(username));
            public UserBuilder WithUsername(Username username) {
                _username = username;
                return this;
            }

            public UserBuilder WithRating(int rating) => WithRating(new Rating(rating));
            public UserBuilder WithRating(Rating rating) {
                _rating = rating;
                return this;
            }

            public UserBuilder WithRank(Rank rank) {
                _rank = rank;
                return this;
            }

            public UserBuilder WithGamesPlayed(int gamesPlayed) {
                _gamesPlayed = gamesPlayed;
                return this;
            }

            public UserBuilder WithCreatedAt(ZonedDateTime createdAt) {
                _createdAt = createdAt;
                return this;
            }

            public User Build() {
                if (_id is null) {
                    throw new InvalidOperationException("Id is not nullable");
                }
                if (_username is null) {
                    throw new InvalidOperationException("Username is not nullable");
                }
                if (_rating is null) {
                    throw new InvalidOperationException("Rating is not nullable");
                }
                if (_rank is null) {
                    throw new InvalidOperationException("Rank is not nullable");
                }
                if (_createdAt is null) {
                    throw new InvalidOperationException("CreatedAt is not nullable");
                }

                return new User(_id, _username, _rating, _rank, _gamesPlayed, _createdAt.Value);
            }
        }
    }
}
