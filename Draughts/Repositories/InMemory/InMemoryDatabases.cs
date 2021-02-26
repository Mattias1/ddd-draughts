using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Database;
using NodaTime;
using System;
using System.Collections.Generic;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;
using static Draughts.Domain.UserAggregate.Models.Rank;

namespace Draughts.Repositories.InMemory {
    public static class UserDatabase {
        public const long AdminId = 1;
        public const long UserId = 2;
        public const long TheColonelId = 3;
        public const long MattyId = 4;
        public const long MathyId = 5;
        public const long JackDeHaasId = 6;
        public const long BobbyId = 7;
        public const long TestPlayerBlack = 8;
        public const long TestPlayerWhite = 9;

        public const long START_FOR_NEXT_IDS = 10;
        public const long START_FOR_NEXT_GAME_IDS = 5;
        public const long START_FOR_NEXT_USER_IDS = 10;

        public static List<DbUser> TempUsersTable { get; } = new List<DbUser>();
        public static List<DbUser> UsersTable { get; } = new List<DbUser> {
            CreateUser(AdminId, "Admin", 1000, Ranks.Private, 0),
            CreateUser(UserId, "User", 1700, Ranks.WarrantOfficer, 7),
            CreateUser(TheColonelId, "TheColonel", 3456, Ranks.Colonel, 37),
            CreateUser(MattyId, "Matty", 2345, Ranks.Lieutenant, 42),
            CreateUser(MathyId, "Mathy", 800, Ranks.LanceCorporal, 12),
            CreateUser(JackDeHaasId, "JackDeHaas", 9001, Ranks.FieldMarshal, 1337),
            CreateUser(BobbyId, "<script>alert('Hi, my name is Bobby');</script>", 1000, Ranks.Private, 0),
            CreateUser(TestPlayerBlack, "TestPlayerBlack", 1000, Ranks.Private, 0),
            CreateUser(TestPlayerWhite, "TestPlayerWhite", 1000, Ranks.Private, 0),
        };

        private static DbUser CreateUser(long id, string name, int rating, Rank rank, int gamesPlayed) {
            if (id >= START_FOR_NEXT_USER_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            var now = SystemClock.Instance.UtcNow();
            return new DbUser {
                Id = id, Username = name, Rating = rating, Rank = rank.Name,
                GamesPlayed = gamesPlayed, CreatedAt = now
            };
        }

        public static List<DomainEvent> TempDomainEventsTable { get; } = new List<DomainEvent>();
        public static List<DomainEvent> DomainEventsTable { get; } = new List<DomainEvent>();
    }

    public static class AuthUserDatabase {
        public static List<DbPermissionRole> TempPermissionRolesTable { get; } = new List<DbPermissionRole>();
        public static List<DbPermissionRole> PermissionRolesTable { get; } = new List<DbPermissionRole>();

        public static List<DbRole> TempRolesTable { get; } = new List<DbRole>();
        public static List<DbRole> RolesTable { get; } = new List<DbRole>();

        public static List<DbAuthUserRole> TempAuthUserRolesTable { get; } = new List<DbAuthUserRole>();
        public static List<DbAuthUserRole> AuthUserRolesTable { get; } = new List<DbAuthUserRole>();

        public static List<DbAuthUser> TempAuthUsersTable { get; } = new List<DbAuthUser>();
        public static List<DbAuthUser> AuthUsersTable { get; } = new List<DbAuthUser> {
        };

        public static List<DomainEvent> TempDomainEventsTable { get; } = new List<DomainEvent>();
        public static List<DomainEvent> DomainEventsTable { get; } = new List<DomainEvent>();

        static AuthUserDatabase() {
            long adminRoleId = 1;
            long pendingRegistrationRoleId = 2;
            long registeredUserRoleId = 3;
            AddRole(adminRoleId, Role.ADMIN_ROLENAME, new[] { Permissions.ViewModPanel.Value,
                Permissions.EditRoles.Value, Permissions.PlayGame.Value });
            AddRole(pendingRegistrationRoleId, Role.PENDING_REGISTRATION_ROLENAME, new[] { Permissions.PendingRegistration.Value });
            AddRole(registeredUserRoleId, Role.REGISTERED_USER_ROLENAME, new[] { Permissions.PlayGame.Value });

            CreateAuthUser(UserDatabase.AdminId, "Admin", adminRoleId, registeredUserRoleId);
            CreateAuthUser(UserDatabase.UserId, "User", registeredUserRoleId);
            CreateAuthUser(UserDatabase.TheColonelId, "TheColonel", registeredUserRoleId);
            CreateAuthUser(UserDatabase.MattyId, "Matty", adminRoleId, registeredUserRoleId);
            CreateAuthUser(UserDatabase.MathyId, "Mathy", registeredUserRoleId);
            CreateAuthUser(UserDatabase.JackDeHaasId, "JackDeHaas", registeredUserRoleId);
            CreateAuthUser(UserDatabase.BobbyId, "<script>alert('Hi, my name is Bobby');</script>", pendingRegistrationRoleId);
            CreateAuthUser(UserDatabase.TestPlayerBlack, "TestPlayerBlack", registeredUserRoleId);
            CreateAuthUser(UserDatabase.TestPlayerWhite, "TestPlayerWhite", registeredUserRoleId);
        }

        private static void AddRole(long id, string name, string[] permissions) {
            if (id >= UserDatabase.START_FOR_NEXT_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            var now = SystemClock.Instance.UtcNow();
            RolesTable.Add(new DbRole {
                Id = id,
                Rolename = name,
                CreatedAt = now
            });
            foreach (string permission in permissions) {
                PermissionRolesTable.Add(new DbPermissionRole {
                    RoleId = id,
                    Permission = permission
                });
            }
        }

        private static void CreateAuthUser(long id, string name, params long[] roleIds) {
            if (id >= UserDatabase.START_FOR_NEXT_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            var hash = PasswordHash.Generate("admin", new UserId(id), new Username(name)).ToStorage();
            var now = SystemClock.Instance.UtcNow();
            AuthUsersTable.Add(new DbAuthUser {
                Id = id,
                Username = name,
                PasswordHash = hash,
                Email = $"{name}@example.com",
                CreatedAt = now
            });
            foreach (long roleId in roleIds) {
                AuthUserRolesTable.Add(new DbAuthUserRole {
                    UserId = id,
                    RoleId = roleId
                });
            }
        }
    }

    public static class GameDatabase {
        public static List<DbGame> TempGamesTable { get; } = new List<DbGame>();
        public static List<DbGame> GamesTable { get; } = new List<DbGame> {
            CreatePendingGame(1, GameSettings.International),
            CreatePendingGame(2, GameSettings.International),
            CreatePendingGame(3, GameSettings.EnglishAmerican),
            CreatePendingGame(4, GameSettings.Mini)
        };

        public static List<DbPlayer> TempPlayersTable { get; } = new List<DbPlayer>();
        public static List<DbPlayer> PlayersTable { get; } = new List<DbPlayer> {
            CreatePlayer(4, 1, UserDatabase.UserId, "User", Color.White, Ranks.WarrantOfficer),
            CreatePlayer(5, 2, UserDatabase.MathyId, "Mathy", Color.Black, Ranks.LanceCorporal),
            CreatePlayer(6, 3, UserDatabase.UserId, "User", Color.Black, Ranks.WarrantOfficer),
            CreatePlayer(7, 4, UserDatabase.MathyId, "Mathy", Color.Black, Ranks.LanceCorporal)
        };

        public static List<DomainEvent> TempDomainEventsTable { get; } = new List<DomainEvent>();
        public static List<DomainEvent> DomainEventsTable { get; } = new List<DomainEvent>();

        private static DbGame CreatePendingGame(long id, GameSettings settings) {
            if (id >= UserDatabase.START_FOR_NEXT_GAME_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_GAME_IDS too low!");
            }
            var now = SystemClock.Instance.UtcNow();
            string capConstraints = settings.CaptureConstraints switch
            {
                GameSettings.DraughtsCaptureConstraints.AnyFinishedSequence => "seq",
                GameSettings.DraughtsCaptureConstraints.MaximumPieces => "max",
                _ => throw new InvalidOperationException("Unknown capture constraint")
            };
            return new DbGame {
                Id = id,
                BoardSize = settings.BoardSize,
                FirstMoveColorIsWhite = settings.FirstMove == Color.White,
                FlyingKings = settings.FlyingKings,
                MenCaptureBackwards = settings.MenCaptureBackwards,
                CaptureConstraints = capConstraints,
                Victor = null,
                CurrentGameState = GameState.InitialState(new GameId(id), settings.BoardSize).StorageString(),
                CaptureSequenceFrom = null,
                CreatedAt = now, StartedAt = null, FinishedAt = null,
                TurnPlayerId = null, TurnCreatedAt = null, TurnExpiresAt = null
            };
        }

        private static DbPlayer CreatePlayer(long id, long gameId, long userId, string name, Color color, Rank rank) {
            if (id >= UserDatabase.START_FOR_NEXT_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            var now = SystemClock.Instance.UtcNow();
            return new DbPlayer {
                Id = id, UserId = userId, Username = name, GameId = gameId,
                Color = color == Color.White, Rank = rank.Name, CreatedAt = now
            };
        }
    }

    public static class MiscDatabase {
        public static List<DbIdGeneration> IdGenerationTable { get; } = new List<DbIdGeneration>(3) {
            new DbIdGeneration("", UserDatabase.START_FOR_NEXT_IDS),
            new DbIdGeneration("game", UserDatabase.START_FOR_NEXT_GAME_IDS),
            new DbIdGeneration("user", UserDatabase.START_FOR_NEXT_USER_IDS),
        };
    }
}
