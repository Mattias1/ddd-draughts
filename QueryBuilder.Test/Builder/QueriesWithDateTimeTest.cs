using FluentAssertions;
using NodaTime;
using NodaTime.Extensions;
using NodaTime.Testing;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;
using SqlQueryBuilder.Test.Fakes;
using Xunit;

namespace SqlQueryBuilder.Test.Builder;

public sealed class QueriesWithDateTimeTest {
    private IInitialQueryBuilder Query() => QueryBuilder.Init(
        new QueryBuilderOptions(new FakeSqlFlavor()) { SmartDate = true, WrapFieldNames = false });
    private IInitialQueryBuilder WithoutSmartDateQuery() => QueryBuilder.Init(
        new QueryBuilderOptions(new FakeSqlFlavor()) { SmartDate = false, WrapFieldNames = false });

    private ZonedClock Clock => FakeClock.FromUtc(2020, 02, 29, 13, 37, 42).InUtc();

    [Fact]
    public void TestSystemDateTimeEqualsParameterized() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").Is(Clock.GetCurrentZonedDateTime().ToDateTimeUtc())
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where created_at = @0");
    }

    [Fact]
    public void TestZonedDateTimeEqualsParameterized() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").Is(Clock.GetCurrentZonedDateTime())
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where created_at = @0");
    }

    [Fact]
    public void TestLocalDateTimeEqualsParameterized() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").Is(Clock.GetCurrentLocalDateTime())
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where created_at = @0");
    }

    [Fact]
    public void TestStringDate() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").Is("2020-02-29")
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at = '2020-02-29'");
    }

    [Fact]
    public void TestSystemDateTimeEquals() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").Is(Clock.GetCurrentZonedDateTime().ToDateTimeUtc())
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at = '2020-02-29 13:37:42'");
    }

    [Fact]
    public void TestZonedDateTimeEquals() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").Is(Clock.GetCurrentZonedDateTime())
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at = '2020-02-29 13:37:42'");
    }

    [Fact]
    public void TestLocalDateTimeEquals() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").Is(Clock.GetCurrentLocalDateTime())
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at = '2020-02-29 13:37:42'");
    }

    [Fact]
    public void TestLtEqDateWithoutSmartOptionsDoesntChangeDate() {
        string sql = WithoutSmartDateQuery().SelectAllFrom("user")
            .Where("created_at").LtEq(Clock.GetCurrentZonedDateTime().Date)
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at <= '2020-02-29'");
    }

    [Fact]
    public void TestLtEqDateWithSmartOptionsChangesDate() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").LtEq(Clock.GetCurrentZonedDateTime().Date)
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at < '2020-03-01'");
    }

    [Fact]
    public void TestGtDateWithoutSmartOptionsDoesntChangeDate() {
        string sql = WithoutSmartDateQuery().SelectAllFrom("user")
            .Where("created_at").Gt(Clock.GetCurrentZonedDateTime().Date)
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at > '2020-02-29'");
    }

    [Fact]
    public void TestGtDateWithSmartOptionsChangesDate() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").Gt(Clock.GetCurrentZonedDateTime().Date)
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at >= '2020-03-01'");
    }

    [Fact]
    public void TestLtEqZonedDateTimeWithSmartOptionsDoesntChangeDate() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").LtEq(Clock.GetCurrentZonedDateTime())
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at <= '2020-02-29 13:37:42'");
    }

    [Fact]
    public void TestLtEqLocalDateTimeWithSmartOptionsDoesntChangeDate() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").LtEq(Clock.GetCurrentLocalDateTime())
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at <= '2020-02-29 13:37:42'");
    }

    [Fact]
    public void TestLtEqSystemDateTimeWithSmartOptionsDoesntChangeDate() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").LtEq(Clock.GetCurrentZonedDateTime().ToDateTimeUnspecified())
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at <= '2020-02-29 13:37:42'");
    }

    [Fact]
    public void TestIsntTime() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").Isnt(Clock.GetCurrentZonedDateTime().TimeOfDay)
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at != '13:37:42'");
    }

    [Fact]
    public void TestLtEqTimeWithSmartOptionsDoesntChangeDate() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").LtEq(Clock.GetCurrentZonedDateTime().TimeOfDay)
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at <= '13:37:42'");
    }

    [Fact]
    public void TestBetweenDateWithSmartOptionsDoesntChangeDate() {
        string sql = Query().SelectAllFrom("user")
            .Where("created_at").Between(Clock.GetCurrentZonedDateTime().Date, Clock.GetCurrentZonedDateTime().Date.PlusDays(4))
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where created_at between '2020-02-29' and '2020-03-04'");
    }
}
