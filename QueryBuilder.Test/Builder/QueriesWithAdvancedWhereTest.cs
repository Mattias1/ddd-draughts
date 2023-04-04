using FluentAssertions;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;
using SqlQueryBuilder.Test.Fakes;
using System;
using Xunit;

namespace SqlQueryBuilder.Test.Builder;

public sealed class QueriesWithAdvancedWhereTest {
    private IInitialQueryBuilder Query() => QueryBuilder.Init(new QueryBuilderOptions(new FakeSqlFlavor()) {
        DontParameterizeNumbers = false,
        WrapFieldNames = false
    });
    private IInitialQueryBuilder OptimizedNumbersQuery() => QueryBuilder.Init(new QueryBuilderOptions(new FakeSqlFlavor()) {
        DontParameterizeNumbers = true,
        WrapFieldNames = false
    });
    private IInitialQueryBuilder WrapFieldNamesQuery() => QueryBuilder.Init(new QueryBuilderOptions(new FakeSqlFlavor()) {
        DontParameterizeNumbers = false,
        WrapFieldNames = true
    });

    [Fact]
    public void TestWhere() {
        string sql = Query().SelectAllFrom("user")
            .Where("username").Is("admin")
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where username = @0");
    }

    [Fact]
    public void TestWhereIsNull() {
        string sql = Query().SelectAllFrom("user")
            .Where("color").IsNull()
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where color is null");
    }

    [Fact]
    public void TestWhereIsntNull() {
        string sql = Query().SelectAllFrom("user")
            .Where("color").IsntNull()
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where color is not null");
    }

    [Fact]
    public void TestWhereEqNull() {
        string sql = Query().SelectAllFrom("user")
            .Where("color").Eq(null)
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where color is null");
    }

    [Fact]
    public void TestWhereIsntRawNull() {
        string sql = Query().SelectAllFrom("user")
            .Where("color").Isnt(null)
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where color is not null");
    }

    [Fact]
    public void TestWhereBetween() {
        string sql = Query().SelectAllFrom("user")
            .Where("number").Between(900, 1100)
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where number between @0 and @1");
    }

    [Fact]
    public void TestWhereNotBetween() {
        string sql = Query().SelectAllFrom("user")
            .Where("number").NotBetween(900, 1100)
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where number not between @0 and @1");
    }

    [Fact]
    public void TestLike() {
        string sql = Query().SelectAllFrom("user")
            .Where("username").Like("%a%")
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where username like @0");
    }

    [Fact]
    public void TestNotLike() {
        string sql = Query().SelectAllFrom("user")
            .Where("username").NotLike("%a%")
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where username not like @0");
    }

    [Fact]
    public void Test2Ands() {
        string sql = Query().SelectAllFrom("user")
            .And("number").Lt(500)
            .And("counter").Gt(50)
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where number < @0 and counter > @1");
    }

    [Fact]
    public void TestOr() {
        string sql = Query().SelectAllFrom("user")
            .Where("counter").Eq(42)
            .Or("counter").Eq(1337)
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where counter = @0 or counter = @1");
    }

    [Fact]
    public void TestBracesAfterAnd() {
        string sql = Query().SelectAllFrom("user")
            .Where(q => q
                .Where("counter").Isnt(42)
                .Or("counter").Isnt(1337)
            )
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where (counter != @0 or counter != @1)");
    }

    [Fact]
    public void TestBracesAfterOr() {
        string sql = Query().SelectAllFrom("user")
            .Where("color").Is("orange")
            .Or(q => q
                .Where("number").GtEq(3000)
                .And("number").LtEq(3600)
            )
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where color = @0 or (number >= @1 and number <= @2)");
    }

    [Fact]
    public void TestRecursiveBraces() {
        string sql = Query().SelectAllFrom("user")
            .Where(q => q.Where(z => z.Where("number").Is(1337)))
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where ((number = @0))");
    }

    [Fact]
    public void TestWhereNot() {
        string sql = Query().SelectAllFrom("user")
            .WhereNot(q => q.Where("color").Is("yellow"))
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where not (color = @0)");
    }

    [Fact]
    public void TestOrNot() {
        string sql = Query().SelectAllFrom("user")
            .Where("username").NotEq("admin")
            .OrNot(q => q.Where("color").Is("orange"))
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where username != @0 or not (color = @1)");
    }

    [Fact]
    public void TestWhereAndOrParameters() {
        string sql = Query().SelectAllFrom("user")
            .Where("counter").Gt(42)
            .And("number").Gt(1337)
            .Or("counter").Gt(1337)
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where counter > 42 and number > 1337 or counter > 1337");
    }

    [Fact]
    public void TestColumnEq() {
        string sql = Query().SelectAll().FromAs("user", "u")
            .FromAs("monarch", "m")
            .Where("u.name").EqColumn("m.name")
            .ToParameterizedSql();
        sql.Should().Be("select * from user as u, monarch as m where u.name = m.name");
    }

    [Fact]
    public void TestColumnNotEq() {
        string sql = Query().SelectAll().FromAs("user", "u")
            .FromAs("monarch", "m")
            .Where("u.name").NotEqColumn("m.name")
            .ToParameterizedSql();
        sql.Should().Be("select * from user as u, monarch as m where u.name != m.name");
    }

    [Fact]
    public void TestColumnGt() {
        string sql = Query().SelectAll().FromAs("user", "u")
            .FromAs("monarch", "m")
            .Where("u.age").GtColumn("m.age")
            .ToParameterizedSql();
        sql.Should().Be("select * from user as u, monarch as m where u.age > m.age");
    }

    [Fact]
    public void TestColumnGtEq() {
        string sql = Query().SelectAll().FromAs("user", "u")
            .FromAs("monarch", "m")
            .Where("u.age").GtEqColumn("m.age")
            .ToParameterizedSql();
        sql.Should().Be("select * from user as u, monarch as m where u.age >= m.age");
    }

    [Fact]
    public void TestColumnLt() {
        string sql = Query().SelectAll().FromAs("user", "u")
            .FromAs("monarch", "m")
            .Where("u.age").LtColumn("m.age")
            .ToParameterizedSql();
        sql.Should().Be("select * from user as u, monarch as m where u.age < m.age");
    }

    [Fact]
    public void TestColumnLtEq() {
        string sql = Query().SelectAll().FromAs("user", "u")
            .FromAs("monarch", "m")
            .Where("u.age").LtEqColumn("m.age")
            .ToParameterizedSql();
        sql.Should().Be("select * from user as u, monarch as m where u.age <= m.age");
    }

    [Fact]
    public void TestWrappedColumnName() {
        string sql = WrapFieldNamesQuery().SelectAll().FromAs("user", "u")
            .FromAs("monarch", "m")
            .Where("u.name").EqColumn("m.name")
            .ToParameterizedSql();
        sql.Should().Be("select * from `user` as `u`, `monarch` as `m` where `u`.`name` = `m`.`name`");
    }

    [Fact]
    public void TestWhereIn() {
        string sql = Query().SelectAllFrom("user")
            .Where("id").In(1, 2, 3)
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where id in (@0, @1, @2)");
    }

    [Fact]
    public void TestWhereInWithOptimizedNumbers() {
        string sql = OptimizedNumbersQuery().SelectAllFrom("user")
            .Where("id").In(1, 2, 3)
            .And("username").Like("%test%")
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where id in (1, 2, 3) and username like @0");
    }

    [Fact]
    public void TestWhereInWithOptimizedBooleans() {
        string sql = OptimizedNumbersQuery().SelectAllFrom("user")
            .Where("is_one").Is(true)
            .And("is_two").Is(false)
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where is_one = 1 and is_two = 0");
    }

    [Fact]
    public void TestWhereInWithValues() {
        string sql = Query().SelectAllFrom("user")
            .Where("username").In("one", "two", "three")
            .ToUnsafeSql();
        sql.Should().Be("select user.* from user where username in ('one', 'two', 'three')");
    }

    [Fact]
    public void TestWhereNotIn() {
        string sql = Query().SelectAllFrom("user")
            .Where("id").Isnt(0)
            .And("id").NotIn(1, 2, 3)
            .ToParameterizedSql();
        sql.Should().Be("select user.* from user where id != @0 and id not in (@1, @2, @3)");
    }

    [Fact]
    public void NoEmptyWhereIn() {
        Action func = () => Query().SelectAllFrom("user")
            .Where("id").In()
            .ToParameterizedSql();
        func.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NoEmptyWhereNotIn() {
        Action func = () => Query().SelectAllFrom("user")
            .Where("id").NotIn()
            .ToParameterizedSql();
        func.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NoOrAsFirst() {
        Action func = () => Query().SelectAllFrom("user")
            .Or("number").Is(1)
            .ToParameterizedSql();
        func.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NoBracesOrAsFirst() {
        Action func = () => Query().SelectAllFrom("user")
            .Or(q => q
                .Where("number").Is(1)
            )
            .ToParameterizedSql();
        func.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NoOrAsFirstBetweenBraces() {
        Action func = () => Query().SelectAllFrom("user")
            .Where("number").Is(1)
            .Or(q => q
                .Or("number").Is(1)
            )
            .ToParameterizedSql();
        func.Should().Throw<InvalidOperationException>();
    }
}
