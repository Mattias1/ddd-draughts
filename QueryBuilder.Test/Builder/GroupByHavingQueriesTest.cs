using FluentAssertions;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;
using SqlQueryBuilder.Test.Fakes;
using Xunit;

namespace SqlQueryBuilder.Test.Builder;

public sealed class GroupByHavingQueriesTest {
    private IInitialQueryBuilder Query() => QueryBuilder.Init(new QueryBuilderOptions(new FakeSqlFlavor()) {
        DontParameterizeNumbers = false
    });

    [Fact]
    public void TestBasicGroupBy() {
        string sql = Query().Select("rank").CountAllAs("ranks").From("user")
            .GroupBy("rank")
            .ToParameterizedSql();
        sql.Should().Be("select `rank`, count(*) as `ranks` from `user` group by `rank`");
    }

    [Fact]
    public void TestGroupByMultipleSelectColumns() {
        string sql = Query().Select("rank").Min("rating").Max("rating").From("user")
            .GroupBy("rank")
            .ToParameterizedSql();
        sql.Should().Be("select `rank`, min(`rating`), max(`rating`) from `user` group by `rank`");
    }

    [Fact]
    public void TestGroupByMultipleGroupColumns() {
        string sql = Query().Select("rank", "games_played").AvgAs("rating", "avg_rating").From("user")
            .GroupBy("rank", "games_played")
            .ToParameterizedSql();
        sql.Should().Be("select `rank`, `games_played`, avg(`rating`) as `avg_rating` from `user` "
            + "group by `rank`, `games_played`");
    }

    [Fact]
    public void TestBasicHaving() {
        string sql = Query().Select("rank").CountAllAs("ranks").From("user")
            .GroupBy("rank")
            .Having("ranks").LtEq(50)
            .OrderByAsc("ranks")
            .ToParameterizedSql();
        sql.Should().Be("select `rank`, count(*) as `ranks` from `user` "
            + "group by `rank` having `ranks` <= @0 order by `ranks` asc");
    }

    [Fact]
    public void TestAdvancedHaving() {
        string sql = Query().SelectAllFrom("user")
            .Having(q => q
                .Having("rating").Gt(2000)
                .AndHaving("games_played").Gt(50)
            )
            .OrHaving(q => q
                .Having("rating").Gt(1500)
                .AndHaving("games_played").Gt(100)
                .AndHaving(p => p
                    .NotHaving(r => r.Having("rank").Like("%private%"))
                    .OrNotHaving(r => r.Having("rank").Like("%protected%"))
                    .AndNotHaving(r => r.Having("rank").Like("%internal%"))
                )
            )
            .OrHaving("rating").Eq(1337)
            .ToParameterizedSql();

        sql.Should().Be("select `user`.* from `user` "
            + "having (`rating` > @0 and `games_played` > @1) "
            + "or (`rating` > @2 and `games_played` > @3 and ("
            + "not (`rank` like @4) or not (`rank` like @5) and not (`rank` like @6))"
            + ") "
            + "or `rating` = @7");
    }
}
