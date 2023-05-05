using Draughts.Repositories.Misc;
using FluentMigrator;
using System.Data;

namespace Draughts.Command.Migrations;

[Tags(DbContext.MISC_DATABASE)]
[Migration(2023_05_05_1703)]
public class Migration_2023_05_05_Misc : AutoReversingMigration {
    private const string SCHEMA = DbContext.MISC_DATABASE;

    public override void Up() {
        Create.Table("id_generation").InSchema(SCHEMA)
            .WithColumn("subject").AsFixedLengthAnsiString(4).NotNullable()
            .WithColumn("available_id").AsInt64().NotNullable();
    }
}
