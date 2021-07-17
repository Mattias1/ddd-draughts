using FluentAssertions;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Exceptions;
using SqlQueryBuilder.Options;
using SqlQueryBuilder.Test.Fakes;
using System;
using System.Collections.Generic;
using Xunit;

namespace SqlQueryBuilder.Test.Builder {
    public class BasicQueriesTest {
        private IInitialQueryBuilder Query() => QueryBuilder.Init(new QueryBuilderOptions(new FakeSqlFlavor()) {
            DontParameterizeNumbers = false
        });

        [Fact]
        public void TestSimpleSelectSql() {
            string sql = Query().SelectAllFrom("user").ToParameterizedSql();
            sql.Should().Be("select `user`.* from `user`");
        }

        [Fact]
        public void TestSimpleOrderBySql() {
            string sql = Query().SelectAllFrom("user")
                .OrderByDesc("rating")
                .ToParameterizedSql();
            sql.Should().Be("select `user`.* from `user` order by `rating` desc");
        }

        [Fact]
        public void TestMultipleOrderBySql() {
            string sql = Query().SelectAllFrom("user")
                .OrderByDesc("rank", "rating").OrderByAsc("games_played", "id")
                .ToParameterizedSql();
            sql.Should().Be("select `user`.* from `user` order by `rank`, `rating` desc, `games_played`, `id` asc");
        }

        [Fact]
        public void TestMultipleOrderByWithRepeatingDescSql() {
            string sql = Query().SelectAllFrom("user")
                .OrderByDesc("rank", "rating").OrderByDesc("games_played").OrderByDesc("id")
                .ToParameterizedSql();
            sql.Should().Be("select `user`.* from `user` order by `rank`, `rating` desc, `games_played` desc, `id` desc");
        }

        [Fact]
        public void TestSimpleInsert() {
            string sql = Query()
                .InsertInto("user")
                .Values(1, 2, "admin", 1000, "Private", 0)
                .ToParameterizedSql();
            sql.Should().Be("insert into `user` values (@0, @1, @2, @3, @4, @5)");
        }

        [Fact]
        public void TestInsertWithColumnNames() {
            string sql = Query()
                .InsertInto("user")
                .Columns("id", "username", "rank")
                .Values(1, "admin", "Private")
                .ToParameterizedSql();
            sql.Should().Be("insert into `user` (`id`, `username`, `rank`) values (@0, @1, @2)");
        }

        [Fact]
        public void TestInsertParameters() {
            string sql = Query()
                .InsertInto("user")
                .Columns("id", "username", "rank")
                .Values(1, "admin", null)
                .ToUnsafeSql();
            sql.Should().Be("insert into `user` (`id`, `username`, `rank`) values (1, 'admin', null)");
        }

        [Fact]
        public void TestInsertMultipleChunks() {
            string sql = Query()
                .InsertInto("user")
                .Columns("id", "username", "rank")
                .Values(1, "admin", null)
                .Values(2, "user", null)
                .ToUnsafeSql();
            sql.Should().Be("insert into `user` (`id`, `username`, `rank`) values (1, 'admin', null), (2, 'user', null)");
        }

        [Fact]
        public void TestInsertFromDictionary() {
            var dict = new Dictionary<string, object?> {
                { "username", "testname" },
                { "rating", 1337 }
            };
            string sql = Query().InsertInto("user").InsertFromDictionary(dict).ToParameterizedSql();
            sql.Should().Be("insert into `user` (`username`, `rating`) values (@0, @1)");
        }

        [Fact]
        public void TestInsertFromModel() {
            var model = new TestUserTable {
                Username = "testname",
                Rating = 1337
            };
            string sql = Query().InsertInto("user").InsertFrom(model).ToParameterizedSql();
            sql.Should().Be("insert into `user` (`username`, `rating`) values (@0, @1)");
        }

        [Fact]
        public void TestInsertFromMultipleModels() {
            var model1 = new TestUserTable {
                Username = "testname",
                Rating = 1337
            };
            var model2 = new TestUserTable {
                Username = "othername",
                Rating = 42
            };
            string sql = Query().InsertInto("user").InsertFrom(model1, model2).ToParameterizedSql();
            sql.Should().Be("insert into `user` (`username`, `rating`) values (@0, @1), (@2, @3)");
        }

        [Fact]
        public void TestUpdate() {
            string sql = Query()
                .Update("user")
                .SetColumn("rating", 1100)
                .SetColumn("rank", "Corporal")
                .SetColumn("games_played", 1)
                .Where("id").Is(1)
                .ToParameterizedSql();
            sql.Should().Be("update `user` set `rating` = @0, `rank` = @1, `games_played` = @2 where `id` = @3");
        }

        [Fact]
        public void TestUpdateParameters() {
            string sql = Query()
                .Update("user")
                .SetColumn("rating", 1100)
                .SetColumn("username", "Miss Piggy")
                .SetColumn("rank", null)
                .SetColumn("games_played", 1)
                .Where("id").Is(2)
                .ToUnsafeSql();
            sql.Should().Be("update `user` set `rating` = 1100, `username` = 'Miss Piggy', "
                + "`rank` = null, `games_played` = 1 where `id` = 2");
        }

        [Fact]
        public void TestUpdateFromDictionary() {
            var dict = new Dictionary<string, object?> {
                { "username", "testname" },
                { "rating", 1337 }
            };
            string sql = Query().Update("user").SetFromDictionary(dict).Where("id").Is(42).ToParameterizedSql();
            sql.Should().Be("update `user` set `username` = @0, `rating` = @1 where `id` = @2");
        }

        [Fact]
        public void TestUpdateFromModel() {
            var model = new TestUserTable {
                Username = "testname",
                Rating = 1337
            };
            string sql = Query().Update("user").SetFrom(model).Where("id").Is(42).ToParameterizedSql();
            sql.Should().Be("update `user` set `username` = @0, `rating` = @1 where `id` = @2");
        }

        [Fact]
        public void TestSimpleDelete() {
            string sql = Query()
                .DeleteFrom("user")
                .Where("id").Is(1)
                .ToParameterizedSql();
            sql.Should().Be("delete from `user` where `id` = @0");
        }

        [Fact]
        public void TestDeleteParameters() {
            string sql = Query()
                .DeleteFrom("user")
                .Where("id").Is(1)
                .ToUnsafeSql();
            sql.Should().Be("delete from `user` where `id` = 1");
        }

        [Fact]
        public void TestPagination() {
            var query = Query().SelectAllFrom("user").Where("id").Gt(42);
            string sqlCount = query.CloneWithoutSelect().CountAll().ToUnsafeSql();
            string sqlList = query.Skip(3).Take(6).ToUnsafeSql();
            sqlCount.Should().Be("select count(*) from `user` where `id` > 42");
            sqlList.Should().Be("select `user`.* from `user` where `id` > 42 take 6 skip 3");
        }

        [Fact]
        public void NoSemicolonAllowedOutsideParameterizedString() {
            Action func = () => Query().SelectAllFrom("user;").Where("username").Eq("test").ToParameterizedSql();
            func.Should().Throw<PotentialSqlInjectionException>();
        }

        [Fact]
        public void NoCommentStringAllowedOutsideParameterizedString() {
            Action func = () => Query().SelectAllFrom("user--").ToParameterizedSql();
            func.Should().Throw<PotentialSqlInjectionException>();
        }

        public class TestUserTable {
            public string? Username { get; set; }
            public int? Rating { get; set; }
        }
    }
}
