using FluentAssertions;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;
using SqlQueryBuilder.Test.Fakes;
using System;
using Xunit;

namespace SqlQueryBuilder.Test.Builder {
    public class QueriesWithAdvancedWhereTest {
        private IInitialQueryBuilder Query() => QueryBuilder.Init(new QueryBuilderOptions(new FakeSqlFlavor()) {
            DontParameterizeNumbers = false
        });
        private IInitialQueryBuilder OptimizedNumbersQuery() => QueryBuilder.Init(new QueryBuilderOptions(new FakeSqlFlavor()) {
            DontParameterizeNumbers = true
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
                .Where("rank").IsNull()
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where rank is null");
        }

        [Fact]
        public void TestWhereIsntNull() {
            string sql = Query().SelectAllFrom("user")
                .Where("rank").IsntNull()
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where rank is not null");
        }

        [Fact]
        public void TestWhereEqNull() {
            string sql = Query().SelectAllFrom("user")
                .Where("rank").Eq(null)
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where rank is null");
        }

        [Fact]
        public void TestWhereNeqNull() {
            string sql = Query().SelectAllFrom("user")
                .Where("rank").Neq(null)
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where rank is not null");
        }

        [Fact]
        public void TestWhereBetween() {
            string sql = Query().SelectAllFrom("user")
                .Where("rating").Between(900, 1100)
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where rating between @0 and @1");
        }

        [Fact]
        public void TestWhereNotBetween() {
            string sql = Query().SelectAllFrom("user")
                .Where("rating").NotBetween(900, 1100)
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where rating not between @0 and @1");
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
                .And("rank").Lt(500)
                .And("games_played").Gt(50)
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where rank < @0 and games_played > @1");
        }

        [Fact]
        public void TestOr() {
            string sql = Query().SelectAllFrom("user")
                .Where("games_played").Eq(42)
                .Or("games_played").Eq(1337)
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where games_played = @0 or games_played = @1");
        }

        [Fact]
        public void TestBracesAfterAnd() {
            string sql = Query().SelectAllFrom("user")
                .Where(q => q
                    .Where("games_played").Isnt(42)
                    .Or("games_played").Isnt(1337)
                )
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where (games_played != @0 or games_played != @1)");
        }

        [Fact]
        public void TestBracesAfterOr() {
            string sql = Query().SelectAllFrom("user")
                .Where("rank").Is("Field marshal")
                .Or(q => q
                    .Where("rating").GtEq(3000)
                    .And("rating").LtEq(3600)
                )
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where rank = @0 or (rating >= @1 and rating <= @2)");
        }

        [Fact]
        public void TestRecursiveBraces() {
            string sql = Query().SelectAllFrom("user")
                .Where(q => q.Where(z => z.Where("rating").Is(1337)))
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where ((rating = @0))");
        }

        [Fact]
        public void TestWhereNot() {
            string sql = Query().SelectAllFrom("user")
                .WhereNot(q => q.Where("rank").Is("Private"))
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where not (rank = @0)");
        }

        [Fact]
        public void TestOrNot() {
            string sql = Query().SelectAllFrom("user")
                .Where("username").NotEq("admin")
                .OrNot(q => q.Where("rank").Is("Private"))
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user where username != @0 or not (rank = @1)");
        }

        [Fact]
        public void TestWhereAndOrParameters() {
            string sql = Query().SelectAllFrom("user")
                .Where("games_played").Gt(42)
                .And("rating").Gt(1337)
                .Or("games_played").Gt(1337)
                .ToUnsafeSql();
            sql.Should().Be("select user.* from user where games_played > 42 and rating > 1337 or games_played > 1337");
        }

        [Fact]
        public void TestColumnEq() {
            string sql = Query().SelectAll().FromAs("user", "u")
                .FromAs("authuser", "a")
                .Where("u.username").EqColumn("a.username")
                .ToParameterizedSql();
            sql.Should().Be("select * from user as u, authuser as a where u.username = a.username");
        }

        [Fact]
        public void TestColumnNotEq() {
            string sql = Query().SelectAll().FromAs("user", "u")
                .FromAs("authuser", "a")
                .Where("u.username").NotEqColumn("a.username")
                .ToParameterizedSql();
            sql.Should().Be("select * from user as u, authuser as a where u.username != a.username");
        }

        [Fact]
        public void TestColumnGt() {
            string sql = Query().SelectAll().FromAs("user", "u")
                .FromAs("authuser", "a")
                .Where("u.username").GtColumn("a.username")
                .ToParameterizedSql();
            sql.Should().Be("select * from user as u, authuser as a where u.username > a.username");
        }

        [Fact]
        public void TestColumnGtEq() {
            string sql = Query().SelectAll().FromAs("user", "u")
                .FromAs("authuser", "a")
                .Where("u.username").GtEqColumn("a.username")
                .ToParameterizedSql();
            sql.Should().Be("select * from user as u, authuser as a where u.username >= a.username");
        }

        [Fact]
        public void TestColumnLt() {
            string sql = Query().SelectAll().FromAs("user", "u")
                .FromAs("authuser", "a")
                .Where("u.username").LtColumn("a.username")
                .ToParameterizedSql();
            sql.Should().Be("select * from user as u, authuser as a where u.username < a.username");
        }

        [Fact]
        public void TestColumnLtEq() {
            string sql = Query().SelectAll().FromAs("user", "u")
                .FromAs("authuser", "a")
                .Where("u.username").LtEqColumn("a.username")
                .ToParameterizedSql();
            sql.Should().Be("select * from user as u, authuser as a where u.username <= a.username");
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
                .Or("rating").Is(1)
                .ToParameterizedSql();
            func.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void NoBracesOrAsFirst() {
            Action func = () => Query().SelectAllFrom("user")
                .Or(q => q
                    .Where("rating").Is(1)
                )
                .ToParameterizedSql();
            func.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void NoOrAsFirstBetweenBraces() {
            Action func = () => Query().SelectAllFrom("user")
                .Where("rating").Is(1)
                .Or(q => q
                    .Or("rating").Is(1)
                )
                .ToParameterizedSql();
            func.Should().Throw<InvalidOperationException>();
        }
    }
}
