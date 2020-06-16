using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
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

        public static List<User> UsersTable { get; } = new List<User> {
            CreateUser(AdminId, "Admin", 1000, Ranks.Private, 0),
            CreateUser(UserId, "User", 1700, Ranks.WarrantOfficer, 7),
            CreateUser(TheColonelId, "TheColonel", 3456, Ranks.Colonel, 37),
            CreateUser(MattyId, "Matty", 2345, Ranks.Lieutenant, 42),
            CreateUser(MathyId, "Mathy", 800, Ranks.LanceCorporal, 12),
            CreateUser(JackDeHaasId, "JackDeHaas", 9001, Ranks.FieldMarshal, 1337),
            CreateUser(BobbyId, "<script>alert('Hi, my name is Bobby');</script>", 1000, Ranks.Private, 0),
        };

        private static User CreateUser(long id, string name, int rating, Rank rank, int gamesPlayed)
            => new User(new UserId(id), new AuthUserId(id), new Username(name), new Rating(rating), rank, gamesPlayed);
    }

    public static class AuthUserDatabase {
        private static readonly Role admin = new Role(new RoleId(1), "Admin",
            Permissions.ViewModPanel, Permissions.EditRoles, Permissions.PlayGame);
        private static readonly Role registeredUser = new Role(new RoleId(2), "Registered user", Permissions.PlayGame);

        public static List<Role> RolesTable { get; } = new List<Role> { admin, registeredUser };

        public static List<AuthUser> AuthUsersTable { get; } = new List<AuthUser> {
            CreateAuthUser(UserDatabase.AdminId, "Admin", admin, registeredUser),
            CreateAuthUser(UserDatabase.UserId, "User", registeredUser),
            CreateAuthUser(UserDatabase.TheColonelId, "TheColonel", registeredUser),
            CreateAuthUser(UserDatabase.MattyId, "Matty", admin, registeredUser),
            CreateAuthUser(UserDatabase.MathyId, "Mathy", registeredUser),
            CreateAuthUser(UserDatabase.JackDeHaasId, "JackDeHaas", registeredUser),
            CreateAuthUser(UserDatabase.BobbyId, "<script>alert('Hi, my name is Bobby');</script>", registeredUser),
        };

        private static AuthUser CreateAuthUser(long id, string name, params Role[] roles) {
            var hash = PasswordHash.Generate("admin", new AuthUserId(id), name);
            return new AuthUser(new AuthUserId(id), new UserId(id), new Username(name), hash, new Email($"{name}@example.com"), roles);
        }
    }
}
