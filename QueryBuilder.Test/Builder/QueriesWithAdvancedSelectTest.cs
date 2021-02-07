using FluentAssertions;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;
using SqlQueryBuilder.Test.Fakes;
using Xunit;

namespace SqlQueryBuilder.Test.Builder {
    public class QueriesWithAdvancedSelect {
        private IInitialQueryBuilder Query() => QueryBuilder.Init(new QueryBuilderOptions(new FakeSqlFlavor()) {
            DontParameterizeNumbers = false
        });

        [Fact]
        public void TestSelectDistinctSql() {
            string sql = Query().SelectDistinct().Column("rank").From("user").ToParameterizedSql();
            sql.Should().Be("select distinct rank from user");
        }

        [Fact]
        public void TestSelectColumnsSql() {
            string sql = Query().Select("id", "username", "rank").From("user").ToParameterizedSql();
            sql.Should().Be("select id, username, rank from user");
        }

        [Fact]
        public void TestSelectColumnsWithAliasSql() {
            string sql = Query().Select()
                .ColumnAs("id", "blah")
                .ColumnAs("username", "blahblah")
                .Column("rank")
                .Column("rating")
                .From("user")
                .ToParameterizedSql();
            sql.Should().Be("select id as blah, username as blahblah, rank, rating from user");
        }

        [Fact]
        public void TestAggregateFunctions() {
            string sql = Query().Select()
                .Column("id")
                .CountAll()
                .Count("admin")
                .Sum("rating")
                .Avg("rating")
                .Max("rating")
                .Min("rating")
                .From("user")
                .ToParameterizedSql();
            sql.Should().Be("select id, count(*), count(admin), sum(rating), avg(rating), max(rating), min(rating) from user");
        }

        [Fact]
        public void TestAggregateFunctionsWithAliases() {
            string sql = Query().Select()
                .Column("id")
                .CountAllAs("rowcount")
                .CountAs("admin", "admins")
                .SumAs("rating", "summ")
                .AvgAs("rating", "avgg")
                .MaxAs("rating", "maxx")
                .MinAs("rating", "minn")
                .From("user")
                .ToParameterizedSql();
            sql.Should().Be("select id, count(*) as rowcount, count(admin) as admins, sum(rating) as summ, " +
                "avg(rating) as avgg, max(rating) as maxx, min(rating) as minn from user");
        }

        [Fact]
        public void TestMultipleFroms() {
            string sql = Query().Select("y.id", "s.id", "s.name")
                .FromAs("user", "u")
                .FromAs("street", "s")
                .Where("u.street_id").IsColumn("s.id")
                .ToParameterizedSql();
            sql.Should().Be("select y.id, s.id, s.name from user as u, street as s where u.street_id = s.id");
        }

        [Fact]
        public void TestInnerJoin() {
            string sql = Query().SelectAllFrom("user")
                .Join("authuser", "user.id", "authuser.user_id")
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user join authuser on user.id = authuser.user_id");
        }

        [Fact]
        public void TestInnerJoinAs() {
            string sql = Query().Select("u.*").FromAs("user", "u")
                .JoinAs("authuser", "a", "u.id", "a.user_id")
                .ToParameterizedSql();
            sql.Should().Be("select u.* from user as u join authuser as a on u.id = a.user_id");
        }

        [Fact]
        public void TestLeftJoin() {
            string sql = Query().SelectAllFrom("user")
                .LeftJoin("authuser", "user.id", "authuser.user_id")
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user left join authuser on user.id = authuser.user_id");
        }

        [Fact]
        public void TestLeftJoinAs() {
            string sql = Query().Select("u.*").FromAs("user", "u")
                .LeftJoinAs("authuser", "a", "u.id", "a.user_id")
                .ToParameterizedSql();
            sql.Should().Be("select u.* from user as u left join authuser as a on u.id = a.user_id");
        }

        [Fact]
        public void TestRightJoin() {
            string sql = Query().SelectAllFrom("user")
                .RightJoin("authuser", "user.id", "authuser.user_id")
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user right join authuser on user.id = authuser.user_id");
        }

        [Fact]
        public void TestRightJoinAs() {
            string sql = Query().Select("u.*").FromAs("user", "u")
                .RightJoinAs("authuser", "a", "u.id", "a.user_id")
                .ToParameterizedSql();
            sql.Should().Be("select u.* from user as u right join authuser as a on u.id = a.user_id");
        }

        [Fact]
        public void TestFullJoin() {
            string sql = Query().SelectAllFrom("user")
                .FullJoin("authuser", "user.id", "authuser.user_id")
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user full join authuser on user.id = authuser.user_id");
        }

        [Fact]
        public void TestFullJoinAs() {
            string sql = Query().Select("u.*").FromAs("user", "u")
                .FullJoinAs("authuser", "a", "u.id", "a.user_id")
                .ToParameterizedSql();
            sql.Should().Be("select u.* from user as u full join authuser as a on u.id = a.user_id");
        }

        [Fact]
        public void TestCrossJoin() {
            string sql = Query().SelectAllFrom("user")
                .CrossJoin("authuser", "user.id", "authuser.user_id")
                .ToParameterizedSql();
            sql.Should().Be("select user.* from user cross join authuser on user.id = authuser.user_id");
        }

        [Fact]
        public void TestCrossJoinAs() {
            string sql = Query().Select("u.*").FromAs("user", "u")
                .CrossJoinAs("authuser", "a", "u.id", "a.user_id")
                .ToParameterizedSql();
            sql.Should().Be("select u.* from user as u cross join authuser as a on u.id = a.user_id");
        }

        [Fact]
        public void TestMultipleJoins() {
            string sql = Query().Select("a.*").FromAs("authuser", "a")
                .JoinAs("authuser_role", "ar", "a.id", "ar.authuser_id")
                .JoinAs("role", "r", "ar.role_id", "r.id")
                .ToParameterizedSql();
            sql.Should().Be("select a.* from authuser as a " +
                "join authuser_role as ar on a.id = ar.authuser_id " +
                "join role as r on ar.role_id = r.id");
        }

        [Fact]
        public void TestQueryBuilderInIncorrectOrder() {
            var q = Query().SelectAllFrom("user u");
            q.Join("some_table s", "s.user_id", "u.id").Where("s.some_column").Is(1).OrderByAsc("s.some_order");
            q.Join("other_table o", "o.user_id", "u.id").Where("o.other_column").Isnt(2).OrderByDesc("o.other_order");
            string sql = q.ToParameterizedSql();

            sql.Should().Be("select u.* from user u " +
                "join some_table s on s.user_id = u.id " +
                "join other_table o on o.user_id = u.id " +
                "where s.some_column = @0 and o.other_column != @1 " +
                "order by s.some_order asc, o.other_order desc");
        }
    }
}
