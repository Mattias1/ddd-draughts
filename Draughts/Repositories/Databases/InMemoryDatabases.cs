using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Databases;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;
using static Draughts.Domain.UserAggregate.Models.Rank;

namespace Draughts.Repositories.Database {
    public static class UserDatabase {
        public const long AdminId = 1;
        public const long UserId = 2;
        public const long TheColonelId = 3;
        public const long MattyId = 4;
        public const long MathyId = 5;
        public const long JackDeHaasId = 6;
        public const long BobbyId = 7;

        public const long START_FOR_NEXT_IDS = 15;

        public static List<InMemoryUser> TempUsersTable { get; } = new List<InMemoryUser>();
        public static List<InMemoryUser> UsersTable { get; } = new List<InMemoryUser> {
            CreateUser(AdminId, "Admin", 1000, Ranks.Private, 0),
            CreateUser(UserId, "User", 1700, Ranks.WarrantOfficer, 7),
            CreateUser(TheColonelId, "TheColonel", 3456, Ranks.Colonel, 37),
            CreateUser(MattyId, "Matty", 2345, Ranks.Lieutenant, 42),
            CreateUser(MathyId, "Mathy", 800, Ranks.LanceCorporal, 12),
            CreateUser(JackDeHaasId, "JackDeHaas", 9001, Ranks.FieldMarshal, 1337),
            CreateUser(BobbyId, "<script>alert('Hi, my name is Bobby');</script>", 1000, Ranks.Private, 0),
        };

        private static InMemoryUser CreateUser(long id, string name, int rating, Rank rank, int gamesPlayed) {
            if (id >= START_FOR_NEXT_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            return new InMemoryUser {
                Id = id, AuthUserId = id, Username = name, Rating = rating, Rank = rank.Name, GamesPlayed = gamesPlayed
            };
        }

        public static List<DomainEvent> TempDomainEventsTable { get; } = new List<DomainEvent>();
        public static List<DomainEvent> DomainEventsTable { get; } = new List<DomainEvent>();
    }

    public static class AuthUserDatabase {
        private static readonly InMemoryRole admin = new InMemoryRole {
            Id = 1, Rolename = Role.ADMIN_ROLENAME, Permissions = new[] {
                Permissions.ViewModPanel.Value, Permissions.EditRoles.Value, Permissions.PlayGame.Value
            }
        };
        private static readonly InMemoryRole pendingRegistration = new InMemoryRole {
            Id = 2, Rolename = Role.PENDING_REGISTRATION_ROLENAME, Permissions = new[] { Permissions.PendingRegistration.Value }
        };
        private static readonly InMemoryRole registeredUser = new InMemoryRole {
            Id = 3, Rolename = Role.REGISTERED_USER_ROLENAME, Permissions = new[] { Permissions.PlayGame.Value }
        };

        public static List<InMemoryRole> TempRolesTable { get; } = new List<InMemoryRole>();
        public static List<InMemoryRole> RolesTable { get; } = new List<InMemoryRole> { admin, pendingRegistration, registeredUser };

        public static List<InMemoryAuthUser> TempAuthUsersTable { get; } = new List<InMemoryAuthUser>();
        public static List<InMemoryAuthUser> AuthUsersTable { get; } = new List<InMemoryAuthUser> {
            CreateAuthUser(UserDatabase.AdminId, "Admin", admin, registeredUser),
            CreateAuthUser(UserDatabase.UserId, "User", registeredUser),
            CreateAuthUser(UserDatabase.TheColonelId, "TheColonel", registeredUser),
            CreateAuthUser(UserDatabase.MattyId, "Matty", admin, registeredUser),
            CreateAuthUser(UserDatabase.MathyId, "Mathy", registeredUser),
            CreateAuthUser(UserDatabase.JackDeHaasId, "JackDeHaas", registeredUser),
            CreateAuthUser(UserDatabase.BobbyId, "<script>alert('Hi, my name is Bobby');</script>", pendingRegistration),
        };

        private static InMemoryAuthUser CreateAuthUser(long id, string name, params InMemoryRole[] roles) {
            if (id >= UserDatabase.START_FOR_NEXT_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            var hash = PasswordHash.Generate("admin", new AuthUserId(id), new Username(name)).ToStorage();
            return new InMemoryAuthUser {
                Id = id, UserId = id, Username = name, PasswordHash = hash, Email = $"{name}@example.com",
                RoleIds = roles.Select(r => r.Id).ToArray()
            };
        }

        public static List<DomainEvent> TempDomainEventsTable { get; } = new List<DomainEvent>();
        public static List<DomainEvent> DomainEventsTable { get; } = new List<DomainEvent>();
    }

    public static class GameDatabase {
        public static List<InMemoryGame> TempGamesTable { get; } = new List<InMemoryGame>();
        public static List<InMemoryGame> GamesTable { get; } = new List<InMemoryGame> {
            CreatePendingGame(8, GameSettings.International, 11),
            CreatePendingGame(9, GameSettings.International, 12),
            CreatePendingGame(10, GameSettings.EnglishAmerican, 13)
        };

        public static List<InMemoryPlayer> TempPlayersTable { get; } = new List<InMemoryPlayer>();
        public static List<InMemoryPlayer> PlayersTable { get; } = new List<InMemoryPlayer> {
            CreatePlayer(11, UserDatabase.UserId, "User", Color.White, Ranks.WarrantOfficer),
            CreatePlayer(12, UserDatabase.MathyId, "Mathy", Color.Black, Ranks.LanceCorporal),
            CreatePlayer(13, UserDatabase.UserId, "User", Color.Black, Ranks.WarrantOfficer)
        };

        public static List<DomainEvent> TempDomainEventsTable { get; } = new List<DomainEvent>();
        public static List<DomainEvent> DomainEventsTable { get; } = new List<DomainEvent>();

        private static InMemoryGame CreatePendingGame(long id, GameSettings settings, long playerId) {
            if (id >= UserDatabase.START_FOR_NEXT_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            var now = SystemClock.Instance.UtcNow();
            return new InMemoryGame {
                Id = id,
                BoardSize = settings.BoardSize,
                FirstMoveColorIsWhite = settings.FirstMove == Color.White,
                FlyingKings = settings.FlyingKings,
                MenCaptureBackwards = settings.MenCaptureBackwards,
                CaptureConstraints = settings.CaptureConstraints,
                CurrentGameState = GameState.InitialState(new GameId(id), settings.BoardSize).ToStorage(),
                CreatedAt = now, StartedAt = null, FinishedAt = null,
                TurnPlayerId = null, TurnCreatedAt = null, TurnExpiresAt = null,
                PlayerIds = new long[] { playerId }
            };
        }

        private static InMemoryPlayer CreatePlayer(long id, long userId, string name, Color color, Rank rank) {
            if (id >= UserDatabase.START_FOR_NEXT_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            return new InMemoryPlayer {
                Id = id, UserId = userId, Username = name,
                ColorIsWhite = color == Color.White, Rank = rank.Name
            };
        }
    }

    public static class MiscDatabase {
        public static List<AvailableId> IdGenerationTable { get; } = new List<AvailableId>(1) {
            new AvailableId { Id = UserDatabase.START_FOR_NEXT_IDS }
        };
    }
}
