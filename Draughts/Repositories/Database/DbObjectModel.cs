using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using NodaTime;
using System;

namespace Draughts.Repositories.Database {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public class DbUser : IDbObject<DbUser, User> {
        public long Id { get; set; }
        public long AuthuserId { get; set; }
        public string Username { get; set; }
        public int Rating { get; set; }
        public string Rank { get; set; }
        public int GamesPlayed { get; set; }
        public ZonedDateTime CreatedAt { get; set; }

        public bool Equals(DbUser? other) => Id.Equals(other?.Id);

        public static DbUser FromDomainModel(User entity) {
            return new DbUser {
                Id = entity.Id,
                AuthuserId = entity.AuthUserId,
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
        public long UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public ZonedDateTime CreatedAt { get; set; }

        public bool Equals(DbAuthUser? other) => Id.Equals(other?.Id);

        public static DbAuthUser FromDomainModel(AuthUser entity) {
            return new DbAuthUser {
                Id = entity.Id,
                UserId = entity.UserId,
                Username = entity.Username,
                PasswordHash = entity.PasswordHash.ToStorage(),
                Email = entity.Email,
                CreatedAt = entity.CreatedAt
            };
        }
    }

    public class DbRole : IDbObject<DbRole, Role> {
        public long Id { get; set; }
        public string Rolename { get; set; }
        public ZonedDateTime CreatedAt { get; set; }

        public bool Equals(DbRole? other) => Id.Equals(other?.Id);

        public static DbRole FromDomainModel(Role entity) {
            return new DbRole {
                Id = entity.Id,
                Rolename = entity.Rolename,
                CreatedAt = entity.CreatedAt
            };
        }
    }

    public class DbAuthUserRole {
        public long AuthuserId { get; set; }
        public long RoleId { get; set; }
    }

    public class DbPermissionRole {
        public long RoleId { get; set; }
        public string Permission { get; set; }
    }

    public class DbGame : IDbObject<DbGame, Game> {
        public long Id { get; set; }
        public int BoardSize { get; set; }
        public bool FirstMoveColorIsWhite { get; set; }
        public bool FlyingKings { get; set; }
        public bool MenCaptureBackwards { get; set; }
        public string CaptureConstraints { get; set; }
        public string CurrentGameState { get; set; }
        public byte? CaptureSequenceFrom { get; set; }
        public ZonedDateTime CreatedAt { get; set; }
        public ZonedDateTime? StartedAt { get; set; }
        public ZonedDateTime? FinishedAt { get; set; }
        public long? TurnPlayerId { get; set; }
        public ZonedDateTime? TurnCreatedAt { get; set; }
        public ZonedDateTime? TurnExpiresAt { get; set; }

        public bool Equals(DbGame? other) => Id.Equals(other?.Id);

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
                CurrentGameState = entity.GameState.StorageString(),
                CaptureSequenceFrom = (byte?)entity.GameState.CaptureSequenceFrom?.Value,
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

    public class DbIdGeneration {
        public long AvailableId { get; set; }

        public bool Equals(DbIdGeneration? other) => AvailableId.Equals(other?.AvailableId);
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
}
