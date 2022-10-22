using Draughts.Application.Shared;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Misc;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime.Testing;
using System.Linq;
using Xunit;

namespace Draughts.Test.Common.Utilities;

public sealed class UtilsTest {
    private const int PAGE_SIZE = 5;
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

    [Fact]
    public void SrcIsSecureAgainstXss() {
        string src = Utils.Src("/script.js", ("version", "<script>alert(1);</script>")).ToString();
        src.Should().Be($"src=\"{BASE_URL}/script.js?version=%3Cscript%3Ealert%281%29%3B%3C%2Fscript%3E\"");
    }

    [Theory]
    [InlineData(2020, 02, 29, 13, 37, "29 Feb 2020, 13:37")]
    [InlineData(2020, 01, 02, 03, 04, "02 Jan 2020, 03:04")]
    public void DateTimeFormatting(int year, int month, int day, int hour, int min, string expected) {
        var clock = FakeClock.FromUtc(year, month, day, hour, min, 0);

        string result = Utils.DateTime(clock.UtcNow());

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(2020, 02, 29, 13, 37, 0, "2020-02-29T13:37:00Z")]
    [InlineData(2020, 1, 2, 3, 4, 5, "2020-01-02T03:04:05Z")]
    public void UtcDateTimeIsoFormatting(int year, int month, int day, int hour, int min, int sec, string expected) {
        var clock = FakeClock.FromUtc(year, month, day, hour, min, sec);

        string result = Utils.DateTimeIso(clock.UtcNow()).ToString();

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 1, "0-0")]
    [InlineData(12, 1, "1-5")]
    [InlineData(12, 2, "6-10")]
    [InlineData(12, 3, "11-12")]
    public void PaginationRange(int nrOfItems, int page, string expected) {
        var viewModel = BuildPaginationViewModel(nrOfItems, page);
        Utils.PaginationRange(viewModel).Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, 1, "0-0 of 0")]
    [InlineData(12, 1, "1-5 of 12")]
    [InlineData(12, 2, "6-10 of 12")]
    [InlineData(12, 3, "11-12 of 12")]
    public void PaginationRangeOfTotal(int nrOfItems, int page, string expected) {
        var viewModel = BuildPaginationViewModel(nrOfItems, page);
        Utils.PaginationRangeOfTotal(viewModel).Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(2, 1, "&lt;,1,2,&gt;")]
    [InlineData(2, 2, "&lt;,1,2,&gt;")]
    [InlineData(9, 1, "&lt;,1,2,3,4,5,6,7,8,9,&gt;")]
    [InlineData(9, 5, "&lt;,1,2,3,4,5,6,7,8,9,&gt;")]
    [InlineData(11, 1, "&lt;,1,2,3,4,5,6,...,10,11,&gt;")]
    [InlineData(11, 2, "&lt;,1,2,3,4,5,6,...,10,11,&gt;")]
    [InlineData(11, 4, "&lt;,1,2,3,4,5,6,...,10,11,&gt;")]
    [InlineData(11, 5, "&lt;,1,2,3,4,5,6,...,10,11,&gt;")]
    [InlineData(11, 6, "&lt;,1,2,...,5,6,7,...,10,11,&gt;")]
    [InlineData(11, 7, "&lt;,1,2,...,6,7,8,9,10,11,&gt;")]
    [InlineData(11, 11, "&lt;,1,2,...,6,7,8,9,10,11,&gt;")]
    [InlineData(42, 11, "&lt;,1,2,...,10,11,12,...,41,42,&gt;")]
    public void PaginationNav(int nrOfPages, int page, string expectedItems) {
        var viewModel = BuildPaginationViewModel(nrOfPages * PAGE_SIZE, page);
        string? nav = Utils.PaginationNav(viewModel, "/", 2, 1, 2).Value;
        nav.Should().Match(MatchString(expectedItems));
    }

    [Fact]
    public void PaginationNavEmptyWhenOnlyOnePage() {
        var viewModel = BuildPaginationViewModel(1 * PAGE_SIZE, 1);
        string? nav = Utils.PaginationNav(viewModel, "/").Value;
        nav.Should().Be("");
    }

    private string MatchString(string pages) => $"*{string.Join('*', pages.Split(',').Select(p => $">{p}<"))}*";

    private IPaginationViewModel<int> BuildPaginationViewModel(int nrOfItems, int currentPage) {
        var items = Enumerable.Range(1, nrOfItems).ToList();
        var pagination = new Pagination<int>(items, nrOfItems, currentPage - 1, PAGE_SIZE);
        return new TestPaginationViewModel(pagination);
    }

    private sealed class TestPaginationViewModel : IPaginationViewModel<int> {
        public Pagination<int> Pagination { get; }
        public TestPaginationViewModel(Pagination<int> pagination) => Pagination = pagination;
    }
}
