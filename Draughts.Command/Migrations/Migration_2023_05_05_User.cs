using Draughts.Repositories.Misc;
using FluentMigrator;

namespace Draughts.Command.Migrations;

[Tags(DbContext.USER_DATABASE)]
[Migration(2023_05_05_1704)]
public class Migration_2023_05_05_User : AutoReversingMigration {
    private const string SCHEMA = DbContext.USER_DATABASE;

    public override void Up() {
        Create.Table("users").InSchema(SCHEMA)
            .WithColumn("id").AsInt64().PrimaryKey().NotNullable()
            .WithColumn("username").AsAnsiString(25).NotNullable().Unique()
            .WithColumn("rating").AsInt32().NotNullable()
            .WithColumn("rank").AsAnsiString(50).NotNullable()
            .WithColumn("total_played").AsInt32().NotNullable()
            .WithColumn("total_won").AsInt32().NotNullable()
            .WithColumn("total_tied").AsInt32().NotNullable()
            .WithColumn("total_lost").AsInt32().NotNullable()
            .WithColumn("international_played").AsInt32().NotNullable()
            .WithColumn("international_won").AsInt32().NotNullable()
            .WithColumn("international_tied").AsInt32().NotNullable()
            .WithColumn("international_lost").AsInt32().NotNullable()
            .WithColumn("english_american_played").AsInt32().NotNullable()
            .WithColumn("english_american_won").AsInt32().NotNullable()
            .WithColumn("english_american_tied").AsInt32().NotNullable()
            .WithColumn("english_american_lost").AsInt32().NotNullable()
            .WithColumn("other_played").AsInt32().NotNullable()
            .WithColumn("other_won").AsInt32().NotNullable()
            .WithColumn("other_tied").AsInt32().NotNullable()
            .WithColumn("other_lost").AsInt32().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable();

        Migration_2023_05_05_Auth.CreateEventsTables(this, SCHEMA);
    }
}
