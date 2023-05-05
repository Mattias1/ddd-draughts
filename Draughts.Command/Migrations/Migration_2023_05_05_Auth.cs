using Draughts.Repositories.Misc;
using FluentMigrator;
using Flurl.Util;
using System.Data;

namespace Draughts.Command.Migrations;

[Tags(DbContext.AUTH_DATABASE)]
[Migration(2023_05_05_1701)]
public class Migration_2023_05_05_Auth : AutoReversingMigration {
    private const string SCHEMA = DbContext.AUTH_DATABASE;

    public override void Up() {
        Create.Table("authusers").InSchema(SCHEMA)
            .WithColumn("id").AsInt64().PrimaryKey().NotNullable()
            .WithColumn("username").AsAnsiString(25).NotNullable().Unique()
            .WithColumn("password_hash").AsAnsiString(200).NotNullable()
            .WithColumn("email").AsAnsiString(200).NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable();

        Create.Table("roles").InSchema(SCHEMA)
            .WithColumn("id").AsInt64().PrimaryKey().NotNullable()
            .WithColumn("rolename").AsAnsiString(50).NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable();

        Create.Table("authuser_roles").InSchema(SCHEMA)
            .WithColumn("user_id").AsInt64().NotNullable()
            .WithColumn("role_id").AsInt64().NotNullable();
        Create.PrimaryKey("pk_authuser_roles")
            .OnTable("authuser_roles").WithSchema(SCHEMA).Columns("user_id", "role_id");
        Create.ForeignKey("fk_aur_au")
            .FromTable("authuser_roles").InSchema(SCHEMA).ForeignColumn("user_id")
            .ToTable("authusers").InSchema(SCHEMA).PrimaryColumn("id")
            .OnUpdate(Rule.None).OnDelete(Rule.Cascade); // In MySQL Rule.None is the same as RESTRICT. So fine for now.
            // Known Limitation of FluentMigrator, see https://github.com/fluentmigrator/fluentmigrator/issues/569.
        Create.ForeignKey("fk_aur_role")
            .FromTable("authuser_roles").InSchema(SCHEMA).ForeignColumn("role_id")
            .ToTable("roles").InSchema(SCHEMA).PrimaryColumn("id")
            .OnUpdate(Rule.None).OnDelete(Rule.Cascade);

        Create.Table("permission_roles").InSchema(SCHEMA)
            .WithColumn("role_id").AsInt64().NotNullable()
            .WithColumn("permission").AsAnsiString(50).NotNullable();
        Create.PrimaryKey("pk_permission_roles")
            .OnTable("permission_roles").WithSchema(SCHEMA).Columns("role_id", "permission");
        Create.ForeignKey("fk_pr_role")
            .FromTable("permission_roles").InSchema(SCHEMA).ForeignColumn("role_id")
            .ToTable("roles").InSchema(SCHEMA).PrimaryColumn("id")
            .OnUpdate(Rule.None).OnDelete(Rule.Cascade);

        Create.Table("adminlog").InSchema(SCHEMA)
            .WithColumn("id").AsInt64().PrimaryKey().NotNullable()
            .WithColumn("type").AsAnsiString(50).NotNullable()
            .WithColumn("parameters").AsAnsiString(50).Nullable()
            .WithColumn("user_id").AsInt64().NotNullable()
            .WithColumn("username").AsAnsiString(25).NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable();
        Create.ForeignKey("fk_al_au")
            .FromTable("adminlog").InSchema(SCHEMA).ForeignColumn("user_id")
            .ToTable("authusers").InSchema(SCHEMA).PrimaryColumn("id")
            .OnUpdate(Rule.None).OnDelete(Rule.Cascade);

        CreateEventsTables(this, SCHEMA);
    }

    public static void CreateEventsTables(MigrationBase migration, string schema) {
        migration.Create.Table("sent_events").InSchema(schema)
            .WithColumn("id").AsInt64().PrimaryKey().NotNullable()
            .WithColumn("type").AsAnsiString(50).NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("handled_at").AsDateTime().Nullable()
            .WithColumn("data").AsString(400).NotNullable();

        migration.Create.Table("received_events").InSchema(schema)
            .WithColumn("id").AsInt64().PrimaryKey().NotNullable()
            .WithColumn("handled_at").AsDateTime().NotNullable();
    }
}
