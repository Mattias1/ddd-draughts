using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using System;

namespace Draughts.Test.TestHelpers {
    public class UserTestHelper {
        public static UserBuilder User(string name = "user") {
            return new UserBuilder()
                .WithId(IdTestHelper.Next())
                .WithAuthUserId(IdTestHelper.Next())
                .WithUsername(name)
                .WithRating(Rating.StartRating)
                .WithRank(Rank.Ranks.Private)
                .WithGamesPlayed(0);
        }


        public class UserBuilder {
            private UserId? _id;
            private AuthUserId? _authUserId;
            private Username? _username;
            private Rating? _rating;
            private Rank? _rank;
            private int _gamesPlayed;

            public UserBuilder WithId(long id) => WithId(new UserId(id));
            public UserBuilder WithId(UserId id) {
                _id = id;
                return this;
            }

            public UserBuilder WithAuthUserId(long authUserId) => WithAuthUserId(new AuthUserId(authUserId));
            public UserBuilder WithAuthUserId(AuthUserId authUserId) {
                _authUserId = authUserId;
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

            public User Build() {
                if (_id is null) {
                    throw new InvalidOperationException("Id is not nullable");
                }
                if (_authUserId is null) {
                    throw new InvalidOperationException("AuthUserId is not nullable");
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

                return new User(_id, _authUserId, _username, _rating, _rank, _gamesPlayed);
            }
        }
    }
}
