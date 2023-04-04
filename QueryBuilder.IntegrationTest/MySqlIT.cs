using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SqlQueryBuilder.IntegrationTest;

public sealed class MySqlIT {
    [Fact]
    public void TestSimpleSelectQuery() {
        var users = DbContext.Get.QueryWithoutTransaction()
            .SelectAllFrom("user")
            .OrderByAsc("id")
            .List<DbUser>();

        users.Select(u => u.Username).Should().BeEquivalentTo("Kermit the Frog", "Miss Piggy", "Elmo");
    }

    [Fact]
    public async Task TestSimpleAsyncSelectQuery() {
        var users = await DbContext.Get.QueryWithoutTransaction()
            .SelectAllFrom("user")
            .OrderByAsc("id")
            .ListAsync<DbUser>();

        users.Select(u => u.Username).Should().BeEquivalentTo("Kermit the Frog", "Miss Piggy", "Elmo");
    }

    [Fact]
    public void TestComplicatedSelectQuery() {
        var name = DbContext.Get.WithTransaction(tran => {
            return DbContext.Get.Query(tran)
                .Select("u.username").FromAs("user", "u")
                .JoinAs("street", "s", "s.id", "u.street_id")
                .Where("s.name").Like("%swamp%")
                .OrderByDesc("u.id")
                .Skip(0).Take(1)
                .SingleString();
        });

        name.Should().Be("Miss Piggy");
    }

    [Fact]
    public async Task TestInsertUpdateAndDelete() {
        using (var tran = await DbContext.Get.BeginTransactionAsync()) {
            await DbContext.Get.Query(tran)
                .InsertInto("user")
                .Values(4, "Grover", 52, "blue", 1337, 2, "1970-05-01 18:00:00")
                .ExecuteAsync();

            await DbContext.Get.Query(tran)
                .Update("user")
                .SetColumn("age", 4)
                .Where("username").Is("Elmo")
                .ExecuteAsync();

            await DbContext.Get.Query(tran)
                .DeleteFrom("user")
                .Where("age").Is(4)
                .ExecuteAsync();

            var usersBeforeRollback = await DbContext.Get.Query(tran)
                .Select("username").From("user").OrderByAsc("id")
                .ListStringsAsync();

            usersBeforeRollback.Should().BeEquivalentTo("Kermit the Frog", "Miss Piggy", "Grover");

            await tran.RollbackAsync();
        }

        var usersAfterRollback = await DbContext.Get.WithTransactionAsync(async tran => {
            return await DbContext.Get.Query(tran)
                .Select("username").From("user").OrderByAsc("id")
                .ListStringsAsync();
        });

        usersAfterRollback.Should().BeEquivalentTo("Kermit the Frog", "Miss Piggy", "Elmo");
    }
}
