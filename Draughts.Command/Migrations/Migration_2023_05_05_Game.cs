using Draughts.Repositories.Misc;
using FluentMigrator;
using System.Data;

namespace Draughts.Command.Migrations;

[Tags(DbContext.GAME_DATABASE)]
[Migration(2023_05_05_1702)]
public class Migration_2023_05_05_Game : AutoReversingMigration {
    private const string SCHEMA = DbContext.GAME_DATABASE;

    public override void Up() {
        Create.Table("games").InSchema(SCHEMA)
            .WithColumn("id").AsInt64().PrimaryKey().NotNullable()
            .WithColumn("board_size").AsByte().NotNullable()
            .WithColumn("first_move_color_is_white").AsBoolean().NotNullable()
            .WithColumn("flying_kings").AsBoolean().NotNullable()
            .WithColumn("men_capture_backwards").AsBoolean().NotNullable()
            .WithColumn("capture_constraints").AsFixedLengthAnsiString(3)
            .WithColumn("turn_time").AsInt32().NotNullable()
            .WithColumn("victor").AsInt64().Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("started_at").AsDateTime().Nullable()
            .WithColumn("finished_at").AsDateTime().Nullable()
            .WithColumn("turn_player_id").AsInt64().Nullable()
            .WithColumn("turn_created_at").AsDateTime().Nullable()
            .WithColumn("turn_expires_at").AsDateTime().Nullable();
        Create.Index().OnTable("games").InSchema(SCHEMA).OnColumn("finished_at");

        Create.Table("players").InSchema(SCHEMA)
            .WithColumn("id").AsInt64().PrimaryKey().NotNullable()
            .WithColumn("user_id").AsInt64().NotNullable()
            .WithColumn("game_id").AsInt64().NotNullable()
            .WithColumn("username").AsAnsiString(25).NotNullable()
            .WithColumn("rank").AsAnsiString(50).NotNullable()
            .WithColumn("color").AsBoolean().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable();
        Create.ForeignKey("fk_p_game")
            .FromTable("players").InSchema(SCHEMA).ForeignColumn("game_id")
            .ToTable("games").InSchema(SCHEMA).PrimaryColumn("id")
            .OnUpdate(Rule.None).OnDelete(Rule.Cascade);
        Create.Index().OnTable("players").InSchema(SCHEMA).OnColumn("user_id");

        Create.Table("gamestates").InSchema(SCHEMA)
            .WithColumn("id").AsInt64().PrimaryKey().NotNullable()
            .WithColumn("initial_game_state").AsAnsiString(72).Nullable();
        Create.ForeignKey("fk_gs_game")
            .FromTable("gamestates").InSchema(SCHEMA).ForeignColumn("id")
            .ToTable("games").InSchema(SCHEMA).PrimaryColumn("id")
            .OnUpdate(Rule.None).OnDelete(Rule.Cascade);

        Create.Table("moves").InSchema(SCHEMA)
            .WithColumn("game_id").AsInt64().NotNullable()
            .WithColumn("index").AsInt16().NotNullable()
            .WithColumn("from").AsByte().NotNullable()
            .WithColumn("to").AsByte().NotNullable()
            .WithColumn("is_capture").AsBoolean().NotNullable();
        Create.PrimaryKey("pk_moves")
            .OnTable("moves").WithSchema(SCHEMA).Columns("game_id", "index");
        Create.ForeignKey("fk_move_gs")
            .FromTable("moves").InSchema(SCHEMA).ForeignColumn("game_id")
            .ToTable("gamestates").InSchema(SCHEMA).PrimaryColumn("id")
            .OnUpdate(Rule.None).OnDelete(Rule.Cascade);

        Create.Table("votes").InSchema(SCHEMA)
            .WithColumn("game_id").AsInt64().NotNullable()
            .WithColumn("user_id").AsInt64().NotNullable()
            .WithColumn("subject").AsAnsiString(10).NotNullable()
            .WithColumn("in_favor").AsBoolean().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable();
        Create.PrimaryKey("pk_votes")
            .OnTable("votes").WithSchema(SCHEMA).Columns("game_id", "user_id", "subject");
        Create.ForeignKey("fk_vote_g")
            .FromTable("votes").InSchema(SCHEMA).ForeignColumn("game_id")
            .ToTable("games").InSchema(SCHEMA).PrimaryColumn("id")
            .OnUpdate(Rule.None).OnDelete(Rule.Cascade);

        Migration_2023_05_05_Auth.CreateEventsTables(this, SCHEMA);
    }
}
