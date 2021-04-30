using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.UserAggregate.Models.Rank;

namespace Draughts.Repositories.Database {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public class DbUser : IDbObject<DbUser, User> {
        public long Id { get; set; }
        public string Username { get; set; }
        public int Rating { get; set; }
        public string Rank { get; set; }
        public int GamesPlayed { get; set; }
        public ZonedDateTime CreatedAt { get; set; }

        public bool Equals(DbUser? other) => Id.Equals(other?.Id);

        public User ToDomainModel() {
            return new User(
                new UserId(Id),
                new Username(Username),
                new Rating(Rating),
                Ranks.All.Single(r => r.Name == Rank),
                GamesPlayed,
                CreatedAt
            );
        }

        public static DbUser FromDomainModel(User entity) {
            return new DbUser {
                Id = entity.Id,
                Username = entity.Username,
                Rating = entity.Rating,
                Rank = entity.Rank.Name,
                GamesPlayed = entity.GamesPlayed,
                CreatedAt = entity.CreatedAt
            };
        }
    }

    public class DbAuthUser : IDbObject<DbAuthUser, AuthUser> {
        public long Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public ZonedDateTime CreatedAt { get; set; }

        public bool Equals(DbAuthUser? other) => Id.Equals(other?.Id);

        public AuthUser ToDomainModel(IEnumerable<RoleId> roles) {
            return new AuthUser(
                new UserId(Id),
                new Username(Username),
                Domain.AuthUserAggregate.Models.PasswordHash.FromStorage(PasswordHash),
                new Email(Email),
                CreatedAt,
                roles
            );
        }

        public static DbAuthUser FromDomainModel(AuthUser entity) {
            return new DbAuthUser {
                Id = entity.Id,
                Username = entity.Username,
                PasswordHash = entity.PasswordHash.ToStorage(),
                Email = entity.Email,
                CreatedAt = entity.CreatedAt
            };
        }
    }

    public class DbAuthUserRole : IEquatable<DbAuthUserRole> {
        public long UserId { get; set; }
        public long RoleId { get; set; }

        public bool Equals(DbAuthUserRole? other) {
            return other is not null && other.UserId == UserId && other.RoleId == RoleId;
        }
    }

    public class DbRole : IDbObject<DbRole, Role> {
        public long Id { get; set; }
        public string Rolename { get; set; }
        public ZonedDateTime CreatedAt { get; set; }

        public bool Equals(DbRole? other) => Id.Equals(other?.Id);

        public Role ToDomainModel(Permission[] permissions) {
            return new Role(new RoleId(Id), Rolename, CreatedAt, permissions);
        }

        public static DbRole FromDomainModel(Role entity) {
            return new DbRole {
                Id = entity.Id,
                Rolename = entity.Rolename,
                CreatedAt = entity.CreatedAt
            };
        }
    }

    public class DbPermissionRole : IEquatable<DbPermissionRole> {
        public long RoleId { get; set; }
        public string Permission { get; set; }

        public bool Equals(DbPermissionRole? other) {
            return other is not null && other.RoleId == RoleId && other.Permission == Permission;
        }
    }

    public class DbAdminLog : IDbObject<DbAdminLog, AdminLog> {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Parameters { get; set; }
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Permission { get; set; }
        public ZonedDateTime CreatedAt { get; set; }

        public bool Equals(DbAdminLog? other) => Id.Equals(other?.Id);

        public AdminLog ToDomainModel() {
            return new AdminLog(
                new AdminLogId(Id),
                Type,
                Parameters.Split(','),
                new UserId(UserId),
                new Username(Username),
                new Permission(Permission),
                CreatedAt
            );
        }

        public static DbAdminLog FromDomainModel(AdminLog entity) {
            return new DbAdminLog {
                Id = entity.Id,
                Type = entity.Type,
                Parameters = string.Join(',', entity.Parameters),
                UserId = entity.UserId,
                Username = entity.Username,
                Permission = entity.Permission.Value,
                CreatedAt = entity.CreatedAt
            };
        }
    }

    public class DbGame : IDbObject<DbGame, Game> {
        public long Id { get; set; }
        public int BoardSize { get; set; }
        public bool FirstMoveColorIsWhite { get; set; }
        public bool FlyingKings { get; set; }
        public bool MenCaptureBackwards { get; set; }
        public string CaptureConstraints { get; set; }
        public long? Victor { get; set; }
        public ZonedDateTime CreatedAt { get; set; }
        public ZonedDateTime? StartedAt { get; set; }
        public ZonedDateTime? FinishedAt { get; set; }
        public long? TurnPlayerId { get; set; }
        public ZonedDateTime? TurnCreatedAt { get; set; }
        public ZonedDateTime? TurnExpiresAt { get; set; }

        public bool Equals(DbGame? other) => Id.Equals(other?.Id);

        public Game ToDomainModel(List<Player> players) {
            return new Game(
                new GameId(Id),
                players,
                GetTurn(players),
                GetGameSettings(),
                players.SingleOrDefault(p => p.UserId == Victor),
                CreatedAt,
                StartedAt,
                FinishedAt
            );
        }

        private Turn? GetTurn(List<Player> players) {
            return TurnPlayerId is null || TurnCreatedAt is null || TurnExpiresAt is null ? null : new Turn(
                players.Single(p => p.Id == TurnPlayerId.Value),
                TurnCreatedAt.Value,
                TurnExpiresAt.Value - TurnCreatedAt.Value
            );
        }

        private GameSettings GetGameSettings() {
            Color firstMoveColor = FirstMoveColorIsWhite ? Color.White : Color.Black;
            var capConstraints = CaptureConstraints switch
            {
                "max" => GameSettings.DraughtsCaptureConstraints.MaximumPieces,
                "seq" => GameSettings.DraughtsCaptureConstraints.AnyFinishedSequence,
                _ => throw new InvalidOperationException("Unknown capture constraint.")
            };
            return new GameSettings(BoardSize, firstMoveColor, FlyingKings, MenCaptureBackwards, capConstraints);
        }

        public static DbGame FromDomainModel(Game entity) {
            string captureConstraints = entity.Settings.CaptureConstraints switch
            {
                GameSettings.DraughtsCaptureConstraints.AnyFinishedSequence => "seq",
                GameSettings.DraughtsCaptureConstraints.MaximumPieces => "max",
                _ => throw new InvalidOperationException("Unknown capture constraint")
            };
            return new DbGame {
                Id = entity.Id,
                BoardSize = entity.Settings.BoardSize,
                FirstMoveColorIsWhite = entity.Settings.FirstMove == Color.White,
                FlyingKings = entity.Settings.FlyingKings,
                MenCaptureBackwards = entity.Settings.MenCaptureBackwards,
                CaptureConstraints = captureConstraints,
                Victor = entity.Victor?.UserId.Id,
                CreatedAt = entity.CreatedAt,
                StartedAt = entity.StartedAt,
                FinishedAt = entity.FinishedAt,
                TurnPlayerId = entity.Turn?.Player.Id.Id,
                TurnCreatedAt = entity.Turn?.CreatedAt,
                TurnExpiresAt = entity.Turn?.ExpiresAt
            };
        }
    }

    public class DbPlayer : IDbObject<DbPlayer, Player> {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long GameId { get; set; }
        public string Username { get; set; }
        public string Rank { get; set; }
        public bool Color { get; set; }
        public ZonedDateTime CreatedAt { get; set; }

        public bool Equals(DbPlayer? other) => Id.Equals(other?.Id);

        public Player ToDomainModel() {
            return new Player(
                new PlayerId(Id),
                new UserId(UserId),
                new Username(Username),
                Ranks.All.Single(r => r.Name == Rank),
                Color ? Domain.GameAggregate.Models.Color.White : Domain.GameAggregate.Models.Color.Black,
                CreatedAt
            );
        }

        public static DbPlayer FromDomainModel(Player entity, GameId gameId) {
            return new DbPlayer {
                Id = entity.Id,
                UserId = entity.UserId,
                GameId = gameId,
                Username = entity.Username,
                Rank = entity.Rank.Name,
                Color = entity.Color == Domain.GameAggregate.Models.Color.White,
                CreatedAt = entity.CreatedAt
            };
        }
    }

    public class DbGameState : IDbObject<DbGameState, GameState> {
        public long Id { get; set; }
        public string CurrentGameState { get; set; }
        public byte? CaptureSequenceFrom { get; set; }

        public bool Equals(DbGameState? other) => Id.Equals(other?.Id);

        public GameState ToDomainModel() {
            return GameState.FromStorage(new GameId(Id), CurrentGameState, CaptureSequenceFrom);
        }
        public static DbGameState FromDomainModel(GameState entity) {
            return new DbGameState {
                Id = entity.Id,
                CurrentGameState = entity.StorageString(),
                CaptureSequenceFrom = (byte?)entity.CaptureSequenceFrom?.Value
            };
        }
    }

    public class DbIdGeneration {
        public const string SUBJECT_MISC = "";
        public const string SUBJECT_GAME = "game";
        public const string SUBJECT_USER = "user";

        public string Subject { get; set; }
        public long AvailableId { get; set; }

        public DbIdGeneration() {}
        public DbIdGeneration(string subject, long id) {
            Subject = subject;
            AvailableId = id;
        }

        public bool Equals(DbIdGeneration? other) => AvailableId.Equals(other?.Subject);
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
}
