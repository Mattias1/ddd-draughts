using FluentAssertions;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Exceptions;
using SqlQueryBuilder.Options;
using SqlQueryBuilder.Test.Fakes;
using System;
using Xunit;

namespace SqlQueryBuilder.Test.Builder;

public sealed class RawQueriesTest {
    private IInitialQueryBuilder Query() => QueryBuilder.Init(new QueryBuilderOptions(new FakeSqlFlavor()) {
        DontParameterizeNumbers = false,
        GuardForForgottenWhere = false
    });

    [Fact]
    public void TestRawSql() {
        string sql = Query().Raw("select * from user").ToParameterizedSql();
        sql.Should().Be("select * from user");
    }

    [Fact]
    public void TestRawSqlParameters() {
        string sql = Query().Select().RawColumn("*, ?", 1337).From("user")
            .RawWhere(" where ?=?", 42, 42)
            .RawOrderBy("age, ? desc", 1337)
            .ToUnsafeSql();
        sql.Should().Be("select *, 1337 from `user` where 42=42 order by age, 1337 desc");
    }

    [Fact]
    public void TestRawInsert() {
        string sql = Query().InsertInto("user")
            .RawInsertColumn("username, age")
            .RawInsertValue("?, 13", "unlucky")
            .ToParameterizedSql();
        sql.Should().Be("insert into `user` (username, age) values (@0, 13)");
    }

    [Fact]
    public void TestRawUpdate() {
        string sql = Query().Update("user").RawSet("username = ?, age = 13", "unlucky").ToParameterizedSql();
        sql.Should().Be("update `user` set username = @0, age = 13");
    }

    [Fact]
    public void TestRawJoin() {
        string sql = Query().SelectAllFromAs("user", "u")
            .JoinAs("role_user", "ru", "u.id", "ru.user_id")
            .RawJoin(" left outer join role r on ru.role_id = r.id")
            .ToParameterizedSql();
        sql.Should().Be("select `u`.* from `user` as `u` "
            + "join `role_user` as `ru` on `u`.`id` = `ru`.`user_id` "
            + "left outer join role r on ru.role_id = r.id");
    }

    [Fact]
    public void TestRawSelect() {
        string sql = Query().SelectDistinct()
            .Column("username").RawColumn(", 42").Column("age")
            .From("user")
            .ToParameterizedSql();
        sql.Should().Be("select distinct `username`, 42, `age` from `user`");
    }

    [Fact]
    public void TestRawWhereMultipleParams() {
        string sql = Query().SelectAllFrom("user")
            .RawWhere(" where counter = ? or counter = ?", 42, 1337)
            .ToParameterizedSql();
        sql.Should().Be("select `user`.* from `user` where counter = @0 or counter = @1");
    }

    [Fact]
    public void TestAdvancedRawWhere() {
        string sql = Query().SelectAllFrom("user")
            .Where("username").Like("%test")
            .Or(q => q
                .RawWhere("1=1").And("username").IsNull()
            ).Or(q => q
                .Where("username").IsNull().RawWhere(" and 2=2")
                .And(p => p.AndNot(r => r.RawWhere("3=3")))
            ).RawWhere(" or (username is null and ?=?)", 4, 4)
            .ToParameterizedSql();
        sql.Should().Be("select `user`.* from `user` " +
            "where `username` like @0 " +
            "or (1=1 and `username` is null) " +
            "or (`username` is null and 2=2 and (not (3=3))) " +
            "or (username is null and @1=@2)");
    }

    [Fact]
    public void TestRawGroupByHaving() {
        string sql = Query().Select("color").CountAll().From("user")
            .RawGroupBy("color")
            .RawHaving(" having count(*) > ?", 10)
            .ToParameterizedSql();
        sql.Should().Be("select `color`, count(*) from `user` group by color having count(*) > @0");
    }

    [Fact]
    public void TestRawOrderBy() {
        string sql = Query().SelectAllFrom("user")
            .RawOrderBy("color asc, age, counter desc")
            .ToParameterizedSql();
        sql.Should().Be("select `user`.* from `user` order by color asc, age, counter desc");
    }

    [Fact]
    public void TestRawLimit() {
        string sql = Query().SelectAllFrom("user")
            .RawLimit(" limit ?, ?", 6, 3)
            .ToParameterizedSql();
        sql.Should().Be("select `user`.* from `user` limit @0, @1");
    }

    [Fact]
    public void NoWrappingCharactersAllowedAsFieldNames() {
        Action func = () => Query().SelectAllFrom("user")
            .Where("hack`the").EqColumn("planet]")
            .ToParameterizedSql();
        func.Should().Throw<PotentialSqlInjectionException>();
    }

    [Fact]
    public void NoSemicolonAllowedOutsideParameterizedString() {
        Action func = () => Query().Raw("select * from user; ").ToParameterizedSql();
        func.Should().Throw<PotentialSqlInjectionException>();
    }

    [Fact]
    public void SemicolonAllowedAsLastCharacter() {
        Action func = () => Query().Raw("select * from user;").ToParameterizedSql();
        func.Should().NotThrow();
    }

    [Fact]
    public void NoCommentStringAllowedOutsideParameterizedString() {
        Action func = () => Query().Raw("select * from user--").ToParameterizedSql();
        func.Should().Throw<PotentialSqlInjectionException>();
    }

    [Fact]
    public void SemicolonAndDashesAllowedInsideParameterizedString() {
        Action func = () => Query().Raw("select * from user where username = ?", "semi;colon--").ToParameterizedSql();
        func.Should().NotThrow();
    }

    [Fact]
    public void SemicolonAndDashesAllowedWhenProtectiveOptionIsTurnedOff() {
        var query = QueryBuilder.Init(new QueryBuilderOptions(new FakeSqlFlavor()) { OverprotectiveSqlInjection = false });
        Action func = () => query.Raw("select * from user;-- ").ToParameterizedSql();
        func.Should().NotThrow();
    }
}
