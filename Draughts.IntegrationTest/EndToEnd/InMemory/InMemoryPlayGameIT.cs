using Draughts.IntegrationTest.EndToEnd.Base;
using System.Threading.Tasks;
using Xunit;

namespace Draughts.IntegrationTest.EndToEnd.InMemory {
    [Collection("PlayGameIT")]
    public class InMemoryPlayGameIT {
        private readonly InMemoryApiTester _apiTester;
        private readonly PlayGameTesterApi<InMemoryApiTester> _gameApi;

        public InMemoryPlayGameIT() {
            _apiTester = new InMemoryApiTester();
            _gameApi = new PlayGameTesterApi<InMemoryApiTester>(_apiTester);
        }

        [Fact]
        public async Task PlayGame() {
            string black = _apiTester.LoginAsTestPlayerBlack();
            await _gameApi.PostCreateGame(black);

            string white = _apiTester.LoginAsTestPlayerWhite();
            await _gameApi.PostJoinGame(white);

            await _gameApi.ViewGamePageWithTurn("white");

            // |_|4|_|4|_|4|        // |_|1|_|2|_|3|
            // |4|_|4|_|4|_|        // |4|_|5|_|6|_|
            // |_|.|_|.|_|.|        // |_|7|_|8|_|9|
            // |.|_|.|_|.|_|        // 10|_11|_12|_|
            // |_|5|_|5|_|5|        // |_13|_14|_15|
            // |5|_|5|_|5|_|        // 16|_17|_18|_|
            await _gameApi.PostMove(13, 11, white);
            await _gameApi.PostMove(6, 9, black);

            await _gameApi.PostMove(11, 8, white);
            await _gameApi.PostMove(5, 12, black);

            // |_|4|_|4|_|4|
            // |4|_|.|_|.|_|
            // |_|.|_|.|_|4|
            // |.|_|.|_|4|_|
            // |_|.|_|5|_|5|
            // |5|_|5|_|5|_|
            await _gameApi.PostMove(15, 8, white);
            await _gameApi.PostMove(2, 6, black);

            await _gameApi.PostMove(16, 13, white);
            await _gameApi.PostMove(6, 11, black);
            await _gameApi.PostMove(11, 16, black);

            // |_|4|_|.|_|4|
            // |4|_|.|_|.|_|
            // |_|.|_|.|_|4|
            // |.|_|.|_|.|_|
            // |_|.|_|5|_|.|
            // |6|_|5|_|5|_|
            await _gameApi.PostMove(14, 12, white);
            await _gameApi.PostMove(9, 14, black);

            await _gameApi.PostMove(18, 11, white);
            await _gameApi.PostMove(16, 8, black);

            // |_|4|_|.|_|4|
            // |4|_|.|_|.|_|
            // |_|.|_|6|_|.|
            // |.|_|.|_|.|_|
            // |_|.|_|.|_|.|
            // |.|_|5|_|.|_|
            await _gameApi.PostMove(17, 13, white);
            await _gameApi.PostMove(8, 16, black);

            await _gameApi.ViewGamePageWithVictor("TestPlayerBlack");

            _gameApi.AssertUserStatisticsAreUpdatedCorrectly();
        }

        [Fact]
        public async Task DrawGame() {
            string black = _apiTester.LoginAsTestPlayerBlack();
            await _gameApi.PostCreateGame(black);

            string white = _apiTester.LoginAsTestPlayerWhite();
            await _gameApi.PostJoinGame(white);

            await _gameApi.PostDrawVote(white);
            await _gameApi.PostDrawVote(black);

            _gameApi.AssertGameIsDraw();
        }
    }
}
