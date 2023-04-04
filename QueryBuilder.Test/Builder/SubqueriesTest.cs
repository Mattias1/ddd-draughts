using FluentAssertions;
using NodaTime;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Test.Fakes;
using Xunit;

namespace SqlQueryBuilder.Test.Builder;

public sealed class SubqueriesTest {
    private IInitialQueryBuilder Query() => QueryBuilder.Init(new FakeSqlFlavor());

    [Fact]
    public void TestWhereExists() {
        string sql = Query().SelectAllFrom("user")
            .WhereExists(q => q
                .SelectAllFrom("statistics")
                .Where("text_statistic").Like("Test")
            )
            .ToUnsafeSql();
        sql.Should().Be("select `user`.* from `user` where exists ("
            + "select `statistics`.* from `statistics` where `text_statistic` like 'Test'"
            + ")");
    }

    [Fact]
    public void TestWhereNotExistsWithValues() {
        string sql = Query().SelectAllFrom("user")
            .WhereNotExists(q => q
                .SelectAllFrom("statistics")
                .Where("text_statistic").Like("Test")
            )
            .ToUnsafeSql();
        sql.Should().Be("select `user`.* from `user` where not exists ("
            + "select `statistics`.* from `statistics` where `text_statistic` like 'Test'"
            + ")");
    }

    [Fact]
    public void TestWhereIn() {
        string sql = Query().SelectAllFrom("user")
            .Where("id").In(q => q
                .Select("user_id")
                .From("statistics")
                .Where("some_statistic").Gt(42)
            )
            .ToParameterizedSql();
        sql.Should().Be("select `user`.* from `user` where `id` in ("
            + "select `user_id` from `statistics` where `some_statistic` > 42"
            + ")");
    }

    [Fact]
    public void TestWhereNotInWithValues() {
        string sql = Query().SelectAllFrom("user")
            .Where("id").NotIn(q => q
                .Select("user_id")
                .From("statistics")
                .Where("text_statistic").Like("Test")
            )
            .ToUnsafeSql();
        sql.Should().Be("select `user`.* from `user` where `id` not in ("
            + "select `user_id` from `statistics` where `text_statistic` like 'Test'"
            + ")");
    }

    [Fact]
    public void TestWhereEqSubquery() {
        string sql = Query().Select("user_id")
            .From("statistics")
            .Where("counter").Eq(q => q
                .Select().Max("counter")
                .From("statistics")
            )
            .ToUnsafeSql();
        sql.Should().Be("select `user_id` from `statistics` where `counter` = ("
            + "select max(`counter`) from `statistics`"
            + ")");
    }

    [Fact]
    public void TestWhereGtSubquery() {
        string sql = Query().Select("user_id")
            .From("statistics")
            .Where("counter").Gt(q => q
                .Select().Avg("counter")
                .From("statistics")
            )
            .ToUnsafeSql();
        sql.Should().Be("select `user_id` from `statistics` where `counter` > ("
            + "select avg(`counter`) from `statistics`"
            + ")");
    }

    [Fact]
    public void TestInsertIntoSubquery() {
        string sql = Query().InsertInto("user_backup")
            .Columns("id", "username", "total_things")
            .Select().Column("user_id").Column("username").SelectSubqueryAs("total_things", q => q
                .Select().CountAll()
                .From("thing")
                .Where("thing.user_id").IsColumn("user.id")
            )
            .From("user")
            .Where("last_active").Gt(new LocalDate(2020, 02, 02))
            .ToUnsafeSql();
        sql.Should().Be("insert into `user_backup` (`id`, `username`, `total_things`) "
            + "select `user_id`, `username`, ("
            + "select count(*) from `thing` where `thing`.`user_id` = `user`.`id`"
            + ") as `total_things` from `user` where `last_active` >= '2020-02-03'");
    }

    [Fact]
    public void TestUpdateFromSubquery() {
        string sql = Query().Update("my_table")
            .SetColumnToColumn("col1", "other.col1")
            .SetColumnToColumn("col2", "other.col2")
            .SetColumn("col3", 0)
            .FromAs("other", q => q
                .Select("col1").Column("col2")
                .From("other_table")
            )
            .Where("my_table.id").IsColumn("other_table.id")
            .ToParameterizedSql();
        sql.Should().Be("update `my_table` "
            + "set `col1` = `other`.`col1`, `col2` = `other`.`col2`, `col3` = 0 "
            + "from ("
            + "select `col1`, `col2` from `other_table`"
            + ") as `other` where `my_table`.`id` = `other_table`.`id`");
    }
}
