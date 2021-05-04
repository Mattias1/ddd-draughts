using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using NodaTime.Testing;
using System;

namespace Draughts.Test.TestHelpers {
    public class PlayerTestHelper {
        private static readonly ZonedDateTime Feb29 = FakeClock.FromUtc(2020, 02, 29).UtcNow();

        public static PlayerBuilder White() {
            return new PlayerBuilder()
                .WithId(IdTestHelper.Next())
                .WithUserId(IdTestHelper.Next())
                .WithUsername("White player")
                .WithRank(Rank.Ranks.Private)
                .WithColor(Color.White)
                .WithCreatedAt(Feb29);
        }

        public static PlayerBuilder Black() {
            return new PlayerBuilder()
                .WithId(IdTestHelper.Next())
                .WithUserId(IdTestHelper.Next())
                .WithUsername("Black player")
                .WithRank(Rank.Ranks.Private)
                .WithColor(Color.Black)
                .WithCreatedAt(Feb29);
        }

        public static PlayerBuilder FromUser(User user) {
            return new PlayerBuilder()
                .WithId(IdTestHelper.Next())
                .WithUserId(user.Id)
                .WithUsername(user.Username)
                .WithRank(user.Rank)
                .WithColor(Color.Black)
                .WithCreatedAt(user.CreatedAt);
        }


        public class PlayerBuilder {
            private PlayerId? _id;
            private UserId? _userId;
            private Username? _username;
            private Rank? _rank;
            private Color? _color;
            private ZonedDateTime? _createdAt;

            public PlayerBuilder WithId(long id) => WithId(new PlayerId(id));
            public PlayerBuilder WithId(PlayerId id) {
                _id = id;
                return this;
            }

            public PlayerBuilder WithUserId(long userId) => WithUserId(new UserId(userId));
            public PlayerBuilder WithUserId(UserId userId) {
                _userId = userId;
                return this;
            }

            public PlayerBuilder WithUsername(string username) => WithUsername(new Username(username));
            public PlayerBuilder WithUsername(Username username) {
                _username = username;
                return this;
            }

            public PlayerBuilder WithRank(Rank rank) {
                _rank = rank;
                return this;
            }

            public PlayerBuilder WithColor(Color color) {
                _color = color;
                return this;
            }

            public PlayerBuilder WithCreatedAt(ZonedDateTime createdAt) {
                _createdAt = createdAt;
                return this;
            }

            public Player Build() {
                if (_id is null) {
                    throw new InvalidOperationException("Id is not nullable");
                }
                if (_userId is null) {
                    throw new InvalidOperationException("UserId is not nullable");
                }
                if (_username is null) {
                    throw new InvalidOperationException("Username is not nullable");
                }
                if (_rank is null) {
                    throw new InvalidOperationException("Rank is not nullable");
                }
                if (_color is null) {
                    throw new InvalidOperationException("Color is not nullable");
                }
                if (_createdAt is null) {
                    throw new InvalidOperationException("CreatedAt is not nullable");
                }

                return new Player(_id, _userId, _username, _rank, _color, _createdAt.Value);
            }
        }
    }
}
