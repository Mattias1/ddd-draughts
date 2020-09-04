using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Test.TestHelpers {
    public class PlayerTestHelper {
        public static PlayerBuilder White() {
            return new PlayerBuilder()
                .WithId(IdTestHelper.Next())
                .WithUserId(IdTestHelper.Next())
                .WithUsername("White player")
                .WithRank(Rank.Ranks.Private)
                .WithColor(Color.White);
        }

        public static PlayerBuilder Black() {
            return new PlayerBuilder()
                .WithId(IdTestHelper.Next())
                .WithUserId(IdTestHelper.Next())
                .WithUsername("Black player")
                .WithRank(Rank.Ranks.Private)
                .WithColor(Color.Black);
        }


        public class PlayerBuilder {
            private PlayerId _id;
            private UserId _userId;
            private Username _username;
            private Rank _rank;
            private Color _color;

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

            public Player Build() {
                return new Player(_id, _userId, _username, _rank, _color);
            }
        }
    }
}
