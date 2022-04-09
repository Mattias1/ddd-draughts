using FluentAssertions;
using NodaTime;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Test.Fakes;
using Xunit;

namespace SqlQueryBuilder.Test.Builder;

public sealed class ReadmeExampleQueriesTest {
    private IInitialQueryBuilder Query() => QueryBuilder.Init(new FakeSqlFlavor());

    [Fact]
    public void TestSearchQuery() {
        string name = "name";

        string sql = Query()
            .SelectAllFrom("user")
            .Where("username").Like($"%{name}%")
            .OrderByDesc("created_at")
            .ToParameterizedSql();

        sql.Should().Be("select `user`.* from `user` where `username` like @0 order by `created_at` desc");
    }

    [Fact]
    public void TestSaveUser() {
        var model = new TestUserModel {
            Id = 42,
            Username = "testname",
            Email = "test@example.com",
            CreatedAt = new LocalDateTime(2020, 02, 29, 13, 37, 42)
        };

        string sql = Query()
            .Update("user")
            .SetFrom(model)
            .Where("id").Is(42)
            .ToParameterizedSql();

        sql.Should().Be("update `user` "
            + "set `id` = 42, `username` = @0, `email` = @1, `created_at` = @2 "
            + "where `id` = 42");
    }

    [Fact]
    public void TestComplicatedQuery() {
        var query = Query()
            .Select().Column("u.id").Column("u.username").CountAs("r.id", "roles")
            .FromAs("user", "u")
            .JoinAs("role_user", "ru", "u.id", "ru.user_id")
            .JoinAs("role", "r", "ru.role_id", "r.id")
            .Where(q => q
                .Where("u.created_at").Gt(new LocalDate(2020, 02, 29))
                .Or("username").Is("moderator") // As benchmark
            )
            .AndNot(q => q
                .Where("u.id").Is(1)
                .Or("u.username").Is("admin")
            )
            .GroupBy("u.id", "u.username")
            .Having("roles").GtEq(3)
            .OrderByAsc("roles");
        string sql = query.ToUnsafeSql();

        sql.Should().Be(""
            + "select `u`.`id`, `u`.`username`, count(`r`.`id`) as `roles` "
            + "from `user` as `u` "
            + "join `role_user` as `ru` on `u`.`id` = `ru`.`user_id` "
            + "join `role` as `r` on `ru`.`role_id` = `r`.`id` "
            + "where ("
                + "`u`.`created_at` >= '2020-03-01' "
                + "or `username` = 'moderator'"
            + ") "
            + "and not ("
                + "`u`.`id` = 1 "
                + "or `u`.`username` = 'admin'"
            + ") "
            + "group by `u`.`id`, `u`.`username` "
            + "having `roles` >= 3 "
            + "order by `roles` asc"
        );
    }

    public sealed class TestUserModel {
        public long? Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public LocalDateTime CreatedAt { get; set; }
    }
}
