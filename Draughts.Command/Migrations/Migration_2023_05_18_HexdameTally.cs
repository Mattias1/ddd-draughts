using Draughts.Repositories.Misc;
using FluentMigrator;
using System.Data;

namespace Draughts.Command.Migrations;

[Tags(DbContext.USER_DATABASE)]
[Migration(2023_05_18_1200)]
public class Migration_2023_05_18_HexdameTally : AutoReversingMigration {
    private const string SCHEMA = DbContext.USER_DATABASE;

    public override void Up() {
        Alter.Table("users").InSchema(SCHEMA)
            .AddColumn("hexdame_played").AsInt32().NotNullable()
            .AddColumn("hexdame_won").AsInt32().NotNullable()
            .AddColumn("hexdame_tied").AsInt32().NotNullable()
            .AddColumn("hexdame_lost").AsInt32().NotNullable();
    }
}
