using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using System;
using System.Collections.Generic;
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

        public const long START_FOR_NEXT_IDS = 10;

        public static List<User> UsersTable { get; } = new List<User> {
            CreateUser(AdminId, "Admin", 1000, Ranks.Private, 0),
            CreateUser(UserId, "User", 1700, Ranks.WarrantOfficer, 7),
            CreateUser(TheColonelId, "TheColonel", 3456, Ranks.Colonel, 37),
            CreateUser(MattyId, "Matty", 2345, Ranks.Lieutenant, 42),
            CreateUser(MathyId, "Mathy", 800, Ranks.LanceCorporal, 12),
            CreateUser(JackDeHaasId, "JackDeHaas", 9001, Ranks.FieldMarshal, 1337),
            CreateUser(BobbyId, "<script>alert('Hi, my name is Bobby');</script>", 1000, Ranks.Private, 0),
        };

        private static User CreateUser(long id, string name, int rating, Rank rank, int gamesPlayed) {
            if (id >= START_FOR_NEXT_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            return new User(new UserId(id), new AuthUserId(id), new Username(name), new Rating(rating), rank, gamesPlayed);
        }
    }

    public static class AuthUserDatabase {
        private static readonly Role admin = new Role(new RoleId(1), Role.ADMIN_ROLENAME,
            Permissions.ViewModPanel, Permissions.EditRoles, Permissions.PlayGame);
        private static readonly Role pendingRegistration = new Role(new RoleId(2), Role.PENDING_REGISTRATION_ROLENAME,
            Permissions.PendingRegistration);
        private static readonly Role registeredUser = new Role(new RoleId(3), Role.REGISTERED_USER_ROLENAME, Permissions.PlayGame);

        public static List<Role> RolesTable { get; } = new List<Role> { admin, pendingRegistration, registeredUser };

        public static List<AuthUser> AuthUsersTable { get; } = new List<AuthUser> {
            CreateAuthUser(UserDatabase.AdminId, "Admin", admin, registeredUser),
            CreateAuthUser(UserDatabase.UserId, "User", registeredUser),
            CreateAuthUser(UserDatabase.TheColonelId, "TheColonel", registeredUser),
            CreateAuthUser(UserDatabase.MattyId, "Matty", admin, registeredUser),
            CreateAuthUser(UserDatabase.MathyId, "Mathy", registeredUser),
            CreateAuthUser(UserDatabase.JackDeHaasId, "JackDeHaas", registeredUser),
            CreateAuthUser(UserDatabase.BobbyId, "<script>alert('Hi, my name is Bobby');</script>", pendingRegistration),
        };

        private static AuthUser CreateAuthUser(long id, string name, params Role[] roles) {
            if (id >= UserDatabase.START_FOR_NEXT_IDS) {
                throw new InvalidOperationException("START_FOR_NEXT_IDS too low!");
            }
            var hash = PasswordHash.Generate("admin", new AuthUserId(id), new Username(name));
            return new AuthUser(new AuthUserId(id), new UserId(id), new Username(name), hash, new Email($"{name}@example.com"), roles);
        }
    }

    public static class MiscDatabase {
        public static List<AvailableId> IdGenerationTable { get; } = new List<AvailableId>(1) {
            new AvailableId { Id = UserDatabase.START_FOR_NEXT_IDS }
        };
    }
}
