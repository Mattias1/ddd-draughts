using Draughts.Application.Shared;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime.Testing;
using Xunit;

namespace Draughts.Test.Common.Utilities {
    public class UtilsTest {
        private const string BASE_URL = "http://localhost:52588";

        [Fact]
        public void InternalUrlExpandsWithBase() {
            Utils.Url("/test?q=1").ToString().Should().Be(BASE_URL + "/test?q=1");
        }

        [Fact]
        public void ExternalUrlDoesNotExpand() {
            Utils.Url("https://example.org/test?q=1").ToString().Should().Be("https://example.org/test?q=1");
        }

        [Fact]
        public void UserLinkFromViewModel() {
            var viewModel = new UserViewModel(UserTestHelper.User("Arthur").WithId(42).Build());
            string link = Utils.UserLink(viewModel).ToString();
            link.Should().Be($"<a class=\"user-a\" href=\"{BASE_URL}/user/42\">Arthur</a>");
        }

        [Fact]
        public void UserLinkFromIdAndName() {
            string link = Utils.UserLink(new UserId(42), new Username("Arthur")).ToString();
            link.Should().Be($"<a class=\"user-a\" href=\"{BASE_URL}/user/42\">Arthur</a>");
        }

        [Fact]
        public void GameLinkUppercaseFromId() {
            string link = Utils.GameLinkU(new GameId(37)).ToString();
            link.Should().Be($"<a href=\"{BASE_URL}/game/37\">Game 37</a>");
        }

        [Fact]
        public void GameLinkLowercaseFromId() {
            string link = Utils.GameLinkL(new GameId(37)).ToString();
            link.Should().Be($"<a href=\"{BASE_URL}/game/37\">game 37</a>");
        }

        [Fact]
        public void HrefIsSecureAgainstXss() {
            string href = Utils.Href("/search?q=<script>alert(1);</script>").ToString();
            href.Should().Be($"href=\"{BASE_URL}/search?q=%3Cscript%3Ealert(1);%3C/script%3E\"");
        }

        [Theory]
        [InlineData(2020, 02, 29, 13, 37, "29 Feb 2020, 13:37")]
        public void DateTimeParsing(int year, int month, int day, int hour, int min, string expected) {
            var clock = FakeClock.FromUtc(year, month, day, hour, min, 0);

            string result = Utils.DateTime(clock.UtcNow());

            result.Should().Be(expected);
        }
    }
}
