using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.AuthUserContext.Models.Permission;
using static Draughts.Domain.UserContext.Models.Rank;

namespace Draughts.Repositories.InMemory {
    public interface IInMemoryDatabase { }

    public class UserDatabase : IInMemoryDatabase {
        public const long AdminId = 1;
        public const long UserId = 2;
        public const long TheColonelId = 3;
        public const long MattyId = 4;
        public const long MathyId = 5;
        public const long JackDeHaasId = 6;
        public const long PendingPlayerId = 7;
        public const long TestPlayerBlack = 8;
        public const long TestPlayerWhite = 9;

        public const long START_FOR_NEXT_IDS = 10;
        public const long START_FOR_NEXT_GAME_IDS = 5;
        public const long START_FOR_NEXT_USER_IDS = 10;

        private static UserDatabase? _instance;
        public static UserDatabase Get => _instance ??= Initialize();

        public List<DbUser> UsersTable { get; }
        public List<DomainEvent> DomainEventsTable { get; }

        private static UserDatabase Initialize() {
            var database = new UserDatabase();

            database.AddUser(AdminId, Username.ADMIN, 1000, Ranks.Private, 0);
            database.AddUser(UserId, "User", 1700, Ranks.WarrantOfficer, 7);
            database.AddUser(TheColonelId, "TheColonel", 3456, Ranks.Colonel, 37);
            database.AddUser(MattyId, Username.MATTY, 2345, Ranks.Lieutenant, 42);
            database.AddUser(MathyId, "Mathy", 800, Ranks.LanceCorporal, 12);
            database.AddUser(JackDeHaasId, "JackDeHaas", 9001, Ranks.FieldMarshal, 1337);
            database.AddUser(PendingPlayerId, "PendingPlayer", 1000, Ranks.Private, 0);
            database.AddUser(TestPlayerBlack, "TestPlayerBlack", 1000, Ranks.Private, 0);
            database.AddUser(TestPlayerWhite, "TestPlayerWhite", 1000, Ranks.Private, 0);

            return database;
        }

        private UserDatabase() {
            UsersTable = new List<DbUser>();
            DomainEventsTable = new List<DomainEvent>();
        }

        private void AddUser(long id, string name, int rating, Rank rank, int gamesPlayed) {
            if (id >= START_FOR_NEXT_USER_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            var now = SystemClock.Instance.UtcNow();
            UsersTable.Add(new DbUser {
                Id = id, Username = name, Rating = rating, Rank = rank.Name,
                GamesPlayed = gamesPlayed, CreatedAt = now
            });
        }

        private static Dictionary<ITransaction, UserDatabase> _tempDatabases = new Dictionary<ITransaction, UserDatabase>();
        public static void CreateTempDatabase(ITransaction transaction) => _tempDatabases[transaction] = new UserDatabase();
        public static void RemoveTempDatabase(ITransaction transaction) => _tempDatabases.Remove(transaction);
        public static UserDatabase Temp(ITransaction transaction) {
            if (_tempDatabases.TryGetValue(transaction, out UserDatabase? database)) {
                return database;
            }
            throw new InvalidOperationException("Transaction is not open - no temp database is created.");
        }
    }

    public class AuthUserDatabase : IInMemoryDatabase {
        private static AuthUserDatabase? _instance;
        public static AuthUserDatabase Get => _instance ??= Initialize();

        public List<DbPermissionRole> PermissionRolesTable { get; }
        public List<DbRole> RolesTable { get; }
        public List<DbAuthUserRole> AuthUserRolesTable { get; }
        public List<DbAuthUser> AuthUsersTable { get; }
        public List<DbAdminLog> AdminLogsTable { get; }
        public List<DomainEvent> DomainEventsTable { get; }

        private static AuthUserDatabase Initialize() {
            var database = new AuthUserDatabase();

            long adminRoleId = 1;
            long pendingRegistrationRoleId = 2;
            long registeredUserRoleId = 3;

            database.AddRole(adminRoleId, Role.ADMIN_ROLENAME,
                Permissions.All.Except(Permissions.IgnoredByAdmins).Select(p => p.Value).ToArray());
            database.AddRole(pendingRegistrationRoleId, Role.PENDING_REGISTRATION_ROLENAME,
                new[] { Permissions.PendingRegistration.Value });
            database.AddRole(registeredUserRoleId, Role.REGISTERED_USER_ROLENAME,
                new[] { Permissions.PlayGame.Value });

            database.AddAuthUser(UserDatabase.AdminId, Username.ADMIN, adminRoleId, registeredUserRoleId);
            database.AddAuthUser(UserDatabase.UserId, "User", registeredUserRoleId);
            database.AddAuthUser(UserDatabase.TheColonelId, "TheColonel", registeredUserRoleId);
            database.AddAuthUser(UserDatabase.MattyId, Username.MATTY, adminRoleId, registeredUserRoleId);
            database.AddAuthUser(UserDatabase.MathyId, "Mathy", registeredUserRoleId);
            database.AddAuthUser(UserDatabase.JackDeHaasId, "JackDeHaas", registeredUserRoleId);
            database.AddAuthUser(UserDatabase.PendingPlayerId, "PendingPlayer", pendingRegistrationRoleId);
            database.AddAuthUser(UserDatabase.TestPlayerBlack, "TestPlayerBlack", registeredUserRoleId);
            database.AddAuthUser(UserDatabase.TestPlayerWhite, "TestPlayerWhite", registeredUserRoleId);

            return database;
        }

        private AuthUserDatabase() {
            PermissionRolesTable = new List<DbPermissionRole>();
            RolesTable = new List<DbRole>();
            AuthUserRolesTable = new List<DbAuthUserRole>();
            AuthUsersTable = new List<DbAuthUser>();
            AdminLogsTable = new List<DbAdminLog>();
            DomainEventsTable = new List<DomainEvent>();
        }

        private void AddRole(long id, string name, string[] permissions) {
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

        private void AddAuthUser(long id, string name, params long[] roleIds) {
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

        private static Dictionary<ITransaction, AuthUserDatabase> _tempDatabases = new Dictionary<ITransaction, AuthUserDatabase>();
        public static void CreateTempDatabase(ITransaction transaction) => _tempDatabases[transaction] = new AuthUserDatabase();
        public static void RemoveTempDatabase(ITransaction transaction) => _tempDatabases.Remove(transaction);
        public static AuthUserDatabase Temp(ITransaction transaction) {
            if (_tempDatabases.TryGetValue(transaction, out AuthUserDatabase? database)) {
                return database;
            }
            throw new InvalidOperationException("Transaction is not open - no temp database is created.");
        }
    }

    public class GameDatabase : IInMemoryDatabase {
        private static GameDatabase? _instance;
        public static GameDatabase Get => _instance ??= Initialize();

        public List<DbGame> GamesTable { get; }
        public List<DbPlayer> PlayersTable { get; }
        public List<DbGameState> GameStatesTable { get; }
        public List<DbMove> MovesTable { get; }
        public List<DomainEvent> DomainEventsTable { get; }

        public static GameDatabase Initialize() {
            var database = new GameDatabase();

            database.AddPendingGameAndState(1, GameSettings.International);
            database.AddPendingGameAndState(2, GameSettings.International);
            database.AddPendingGameAndState(3, GameSettings.EnglishAmerican);
            database.AddPendingGameAndState(4, GameSettings.Mini);

            database.AddPlayer(4, 1, UserDatabase.UserId, "User", Color.White, Ranks.WarrantOfficer);
            database.AddPlayer(5, 2, UserDatabase.MathyId, "Mathy", Color.Black, Ranks.LanceCorporal);
            database.AddPlayer(6, 3, UserDatabase.UserId, "User", Color.Black, Ranks.WarrantOfficer);
            database.AddPlayer(7, 4, UserDatabase.MathyId, "Mathy", Color.Black, Ranks.LanceCorporal);

            return database;
        }

        private GameDatabase() {
            GamesTable = new List<DbGame>();
            PlayersTable = new List<DbPlayer>();
            GameStatesTable = new List<DbGameState>();
            MovesTable = new List<DbMove>();
            DomainEventsTable = new List<DomainEvent>();
        }

        private void AddPendingGameAndState(long id, GameSettings settings) {
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
            GamesTable.Add(new DbGame {
                Id = id,
                BoardSize = settings.BoardSize,
                FirstMoveColorIsWhite = settings.FirstMove == Color.White,
                FlyingKings = settings.FlyingKings,
                MenCaptureBackwards = settings.MenCaptureBackwards,
                CaptureConstraints = capConstraints,
                Victor = null,
                CreatedAt = now, StartedAt = null, FinishedAt = null,
                TurnPlayerId = null, TurnCreatedAt = null, TurnExpiresAt = null
            });
            GameStatesTable.Add(new DbGameState {
                Id = id,
                InitialGameState = null
            });
        }

        private void AddPlayer(long id, long gameId, long userId, string name, Color color, Rank rank) {
            if (id >= UserDatabase.START_FOR_NEXT_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            var now = SystemClock.Instance.UtcNow();
            PlayersTable.Add(new DbPlayer {
                Id = id,
                UserId = userId,
                Username = name,
                GameId = gameId,
                Color = color == Color.White,
                Rank = rank.Name,
                CreatedAt = now
            });
        }

        private static Dictionary<ITransaction, GameDatabase> _tempDatabases = new Dictionary<ITransaction, GameDatabase>();
        public static void CreateTempDatabase(ITransaction transaction) => _tempDatabases[transaction] = new GameDatabase();
        public static void RemoveTempDatabase(ITransaction transaction) => _tempDatabases.Remove(transaction);
        public static GameDatabase Temp(ITransaction transaction) {
            if (_tempDatabases.TryGetValue(transaction, out GameDatabase? database)) {
                return database;
            }
            throw new InvalidOperationException("Transaction is not open - no temp database is created.");
        }
    }

    public class MiscDatabase : IInMemoryDatabase {
        private static MiscDatabase? _instance;
        public static MiscDatabase Get => _instance ??= Initialize();

        public List<DbIdGeneration> IdGenerationTable { get; }

        private static MiscDatabase Initialize() {
            var database = new MiscDatabase();

            database.IdGenerationTable.Add(new DbIdGeneration("", UserDatabase.START_FOR_NEXT_IDS));
            database.IdGenerationTable.Add(new DbIdGeneration("game", UserDatabase.START_FOR_NEXT_GAME_IDS));
            database.IdGenerationTable.Add(new DbIdGeneration("user", UserDatabase.START_FOR_NEXT_USER_IDS));

            return database;
        }

        private MiscDatabase() {
            IdGenerationTable = new List<DbIdGeneration>();
        }
    }
}
