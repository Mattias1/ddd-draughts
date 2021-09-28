using FluentAssertions;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Test.Fakes;
using Xunit;

namespace SqlQueryBuilder.Test.Builder {
    public class SubqueriesTest {
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
                .Where("total_won").Eq(q => q
                    .Select().Max("total_won")
                    .From("statistics")
                )
                .ToUnsafeSql();
            sql.Should().Be("select `user_id` from `statistics` where `total_won` = ("
                + "select max(`total_won`) from `statistics`"
                + ")");
        }

        [Fact]
        public void TestWhereGtSubquery() {
            string sql = Query().Select("user_id")
                .From("statistics")
                .Where("total_won").Gt(q => q
                    .Select().Avg("total_won")
                    .From("statistics")
                )
                .ToUnsafeSql();
            sql.Should().Be("select `user_id` from `statistics` where `total_won` > ("
                + "select avg(`total_won`) from `statistics`"
                + ")");
        }
    }
}
