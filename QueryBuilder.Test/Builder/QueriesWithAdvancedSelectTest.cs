using FluentAssertions;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;
using SqlQueryBuilder.Test.Fakes;
using Xunit;

namespace SqlQueryBuilder.Test.Builder;

public sealed class QueriesWithAdvancedSelect {
    private IInitialQueryBuilder Query() => QueryBuilder.Init(new QueryBuilderOptions(new FakeSqlFlavor()) {
        DontParameterizeNumbers = false
    });

    [Fact]
    public void TestSelectDistinctSql() {
        string sql = Query().SelectDistinct().Column("color").From("user").ToParameterizedSql();
        sql.Should().Be("select distinct `color` from `user`");
    }

    [Fact]
    public void TestSelectColumnsSql() {
        string sql = Query().Select("id", "username", "color").From("user").ToParameterizedSql();
        sql.Should().Be("select `id`, `username`, `color` from `user`");
    }

    [Fact]
    public void TestSelectColumnsWithAliasSql() {
        string sql = Query().Select()
            .ColumnAs("id", "blah")
            .ColumnAs("username", "blahblah")
            .Column("color")
            .Column("age")
            .From("user")
            .ToParameterizedSql();
        sql.Should().Be("select `id` as `blah`, `username` as `blahblah`, `color`, `age` from `user`");
    }

    [Fact]
    public void TestAggregateFunctions() {
        string sql = Query().Select()
            .Column("id")
            .CountAll()
            .Count("admin")
            .Sum("age")
            .Avg("age")
            .Max("age")
            .Min("age")
            .From("user")
            .ToParameterizedSql();
        sql.Should().Be("select `id`, count(*), count(`admin`), sum(`age`), avg(`age`), "
            + "max(`age`), min(`age`) from `user`");
    }

    [Fact]
    public void TestAggregateFunctionsWithAliases() {
        string sql = Query().Select()
            .Column("id")
            .CountAllAs("rowcount")
            .CountAs("admin", "admins")
            .SumAs("age", "summ")
            .AvgAs("age", "avgg")
            .MaxAs("age", "maxx")
            .MinAs("age", "minn")
            .From("user")
            .ToParameterizedSql();
        sql.Should().Be("select `id`, count(*) as `rowcount`, count(`admin`) as `admins`, sum(`age`) as `summ`, "
            + "avg(`age`) as `avgg`, max(`age`) as `maxx`, min(`age`) as `minn` from `user`");
    }

    [Fact]
    public void TestMultipleFroms() {
        string sql = Query().Select("y.id", "s.id", "s.name")
            .FromAs("user", "u")
            .FromAs("street", "s")
            .Where("u.street_id").IsColumn("s.id")
            .ToParameterizedSql();
        sql.Should().Be("select `y`.`id`, `s`.`id`, `s`.`name` "
            + "from `user` as `u`, `street` as `s` where `u`.`street_id` = `s`.`id`");
    }

    [Fact]
    public void TestInnerJoin() {
        string sql = Query().SelectAllFrom("user")
            .Join("street", "user.street_id", "street.id")
            .ToParameterizedSql();
        sql.Should().Be("select `user`.* from `user` join `street` on `user`.`street_id` = `street`.`id`");
    }

    [Fact]
    public void TestInnerJoinAs() {
        string sql = Query().Select("u.*").FromAs("user", "u")
            .JoinAs("street", "s", "u.street_id", "s.id")
            .ToParameterizedSql();
        sql.Should().Be("select `u`.* from `user` as `u` join `street` as `s` on `u`.`street_id` = `s`.`id`");
    }

    [Fact]
    public void TestLeftJoin() {
        string sql = Query().SelectAllFrom("user")
            .LeftJoin("street", "user.street_id", "street.id")
            .ToParameterizedSql();
        sql.Should().Be("select `user`.* from `user` left join `street` on `user`.`street_id` = `street`.`id`");
    }

    [Fact]
    public void TestLeftJoinAs() {
        string sql = Query().Select("u.*").FromAs("user", "u")
            .LeftJoinAs("street", "s", "u.street_id", "s.id")
            .ToParameterizedSql();
        sql.Should().Be("select `u`.* from `user` as `u` left join `street` as `s` on `u`.`street_id` = `s`.`id`");
    }

    [Fact]
    public void TestRightJoin() {
        string sql = Query().SelectAllFrom("user")
            .RightJoin("street", "user.street_id", "street.id")
            .ToParameterizedSql();
        sql.Should().Be("select `user`.* from `user` right join `street` on `user`.`street_id` = `street`.`id`");
    }

    [Fact]
    public void TestRightJoinAs() {
        string sql = Query().Select("u.*").FromAs("user", "u")
            .RightJoinAs("street", "s", "u.street_id", "s.id")
            .ToParameterizedSql();
        sql.Should().Be("select `u`.* from `user` as `u` right join `street` as `s` on `u`.`street_id` = `s`.`id`");
    }

    [Fact]
    public void TestFullJoin() {
        string sql = Query().SelectAllFrom("user")
            .FullJoin("street", "user.street_id", "street.id")
            .ToParameterizedSql();
        sql.Should().Be("select `user`.* from `user` full join `street` on `user`.`street_id` = `street`.`id`");
    }

    [Fact]
    public void TestFullJoinAs() {
        string sql = Query().Select("u.*").FromAs("user", "u")
            .FullJoinAs("street", "s", "u.street_id", "s.id")
            .ToParameterizedSql();
        sql.Should().Be("select `u`.* from `user` as `u` full join `street` as `s` on `u`.`street_id` = `s`.`id`");
    }

    [Fact]
    public void TestCrossJoin() {
        string sql = Query().SelectAllFrom("user")
            .CrossJoin("street", "user.street_id", "street.id")
            .ToParameterizedSql();
        sql.Should().Be("select `user`.* from `user` cross join `street` on `user`.`street_id` = `street`.`id`");
    }

    [Fact]
    public void TestCrossJoinAs() {
        string sql = Query().Select("u.*").FromAs("user", "u")
            .CrossJoinAs("street", "s", "u.street_id", "s.id")
            .ToParameterizedSql();
        sql.Should().Be("select `u`.* from `user` as `u` cross join `street` as `s` on `u`.`street_id` = `s`.`id`");
    }

    [Fact]
    public void TestMultipleJoins() {
        string sql = Query().Select("s.*").ColumnAs("sa.name", "historical_name").FromAs("street", "s")
            .JoinAs("street_audit", "sa", "s.id", "sa.id")
            .JoinAs("user", "u", "sa.named_after", "u.id")
            .Where("u.name").Like("%Hitler%")
            .ToParameterizedSql();
        sql.Should().Be("select `s`.*, `sa`.`name` as `historical_name` from `street` as `s` "
            + "join `street_audit` as `sa` on `s`.`id` = `sa`.`id` "
            + "join `user` as `u` on `sa`.`named_after` = `u`.`id` "
            + "where `u`.`name` like @0");
    }

    [Fact]
    public void TestQueryBuilderInIncorrectOrder() {
        var q = Query().SelectAllFromAs("user", "u");
        q.JoinAs("some_table", "s", "s.user_id", "u.id").Where("s.some_column").Is(1).OrderByAsc("s.some_order");
        q.JoinAs("other_table", "o", "o.user_id", "u.id").Where("o.other_column").Isnt(2).OrderByDesc("o.other_order");
        string sql = q.ToParameterizedSql();

        sql.Should().Be("select `u`.* from `user` as `u` "
            + "join `some_table` as `s` on `s`.`user_id` = `u`.`id` "
            + "join `other_table` as `o` on `o`.`user_id` = `u`.`id` "
            + "where `s`.`some_column` = @0 and `o`.`other_column` != @1 "
            + "order by `s`.`some_order` asc, `o`.`other_order` desc");
    }
}
