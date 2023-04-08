using FluentAssertions;
using NodaTime;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SqlQueryBuilder.IntegrationTest;

public sealed class MySqlIT {
    [Fact]
    public void TestSimpleSelectQuery() {
        var users = DbContext.MySql.QueryWithoutTransaction()
            .SelectAllFrom("user")
            .OrderByAsc("id")
            .List<DbUser>();

        users.Select(u => u.Username).Should().BeEquivalentTo("Kermit the Frog", "Miss Piggy", "Elmo");
    }

    [Fact]
    public async Task TestSimpleAsyncSelectQuery() {
        var users = await DbContext.MySql.QueryWithoutTransaction()
            .SelectAllFrom("user")
            .OrderByAsc("id")
            .ListAsync<DbUser>();

        users.Select(u => u.Username).Should().BeEquivalentTo("Kermit the Frog", "Miss Piggy", "Elmo");
    }

    [Fact]
    public void TestComplicatedSelectQuery() {
        using var tran = DbContext.MySql.BeginTransaction();

        var name = DbContext.MySql.Query(tran)
            .Select("u.username").FromAs("user", "u")
            .JoinAs("street", "s", "s.id", "u.street_id")
            .Where("s.name").Like("%swamp%")
            .OrderByDesc("u.id")
            .Skip(0).Take(1)
            .SingleString();

        tran.Commit();

        name.Should().Be("Miss Piggy");
    }

    [Fact]
    public void TestSelectNodaTimeProperty() {
        var elmosBirthday = DbContext.MySql.QueryWithoutTransaction()
            .Select("created_at").From("user")
            .Where("username").Is("Elmo")
            .Single<LocalDateTime>();

        elmosBirthday.Should().Be(new LocalDateTime(1980, 2, 3, 18, 0));
    }

    [Fact]
    public void TestSnakeCaseMapping() {
        var kermit = DbContext.MySql.QueryWithoutTransaction()
            .SelectAllFrom("user")
            .OrderByAsc("id")
            .Where("id").Is(1)
            .Single<DbUser>();

        kermit.StreetId.Should().Be(1);
    }

    [Fact]
    public async Task TestInsertUpdateAndDelete() {
        using (var tran = await DbContext.MySql.BeginTransactionAsync()) {
            await DbContext.MySql.Query(tran)
                .InsertInto("user")
                .Values(4, "Grover", 52, "blue", 1337, 2, "1970-05-01 18:00:00")
                .ExecuteAsync();

            await DbContext.MySql.Query(tran)
                .Update("user")
                .SetColumn("age", 4)
                .Where("username").Is("Elmo")
                .ExecuteAsync();

            await DbContext.MySql.Query(tran)
                .DeleteFrom("user")
                .Where("age").Is(4)
                .ExecuteAsync();

            var usersBeforeRollback = await DbContext.MySql.Query(tran)
                .Select("username").From("user").OrderByAsc("id")
                .ListStringsAsync();

            usersBeforeRollback.Should().BeEquivalentTo("Kermit the Frog", "Miss Piggy", "Grover");

            await tran.RollbackAsync();
        }

        using (var tran = await DbContext.MySql.BeginTransactionAsync()) {
            var usersAfterRollback = await DbContext.MySql.Query(tran)
                .Select("username").From("user").OrderByAsc("id")
                .ListStringsAsync();

            await tran.CommitAsync();

            usersAfterRollback.Should().BeEquivalentTo("Kermit the Frog", "Miss Piggy", "Elmo");
        }
    }
}
