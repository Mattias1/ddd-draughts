using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using NodaTime.Testing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Test.TestHelpers;

public sealed class AuthUserTestHelper {
    private static readonly ZonedDateTime Feb29 = FakeClock.FromUtc(2020, 02, 29).UtcNow();

    public static AuthUserBuilder FromUser(User user) {
        var registeredUserRole = RoleTestHelper.RegisteredUser().Build();
        return FromUserAndRoles(user, registeredUserRole);
    }

    public static AuthUserBuilder FromUserAndRoles(User user, params Role[] roles) {
        return new AuthUserBuilder()
            .WithId(user.Id)
            .WithUsername(user.Username)
            .WithEmail($"{user.Username}@example.com")
#if DEBUG
            .WithPassword(user.Username.Value)
#else
            .WithPassword("ChangeMe :)")
#endif
            .WithCreatedAt(user.CreatedAt)
            .WithRoles(roles);
    }

    public static AuthUserBuilder User(string name = "user") {
        var registeredUserRole = RoleTestHelper.RegisteredUser().Build();

        return new AuthUserBuilder()
            .WithId(IdTestHelper.NextForUser())
            .WithUsername(new Username(name))
            .WithEmail($"{name}@example.com")
            .WithPassword(name)
            .WithCreatedAt(Feb29)
            .WithRoles(registeredUserRole);
    }


    public sealed class AuthUserBuilder {
        private UserId? _id;
        private Username? _username;
        private PasswordHash? _passwordHash;
        private Email? _email;
        private ZonedDateTime? _createdAt;
        private List<RoleId> _roleIds = new List<RoleId>();

        public AuthUserBuilder WithId(long id) => WithId(new UserId(id));
        public AuthUserBuilder WithId(UserId id) {
            _id = id;
            return this;
        }

        public AuthUserBuilder WithUsername(string username) => WithUsername(new Username(username));
        public AuthUserBuilder WithUsername(Username username) {
            _username = username;
            return this;
        }

        public AuthUserBuilder WithPassword(string plaintextPassword) {
            if (_id is null || _username is null) {
                throw new InvalidOperationException("The id and username should be set (and not changed)");
            }
            return WithPasswordHash(PasswordHash.Generate(plaintextPassword, _id, _username));
        }
        public AuthUserBuilder WithPasswordHash(PasswordHash passwordHash) {
            _passwordHash = passwordHash;
            return this;
        }

        public AuthUserBuilder WithEmail(string email) => WithEmail(new Email(email));
        public AuthUserBuilder WithEmail(Email email) {
            _email = email;
            return this;
        }

        public AuthUserBuilder WithCreatedAt(ZonedDateTime createdAt) {
            _createdAt = createdAt;
            return this;
        }

        public AuthUserBuilder WithRoles(params Role[] roles) => WithRoles(roles.Select(r => r.Id));
        public AuthUserBuilder WithRoles(List<Role> roles) => WithRoles(roles.Select(r => r.Id));
        public AuthUserBuilder WithRoles(IEnumerable<RoleId> roleIds) {
            _roleIds = roleIds.ToList();
            return this;
        }
        public AuthUserBuilder AddRole(Role role) => AddRole(role.Id);
        public AuthUserBuilder AddRole(RoleId roleId) {
            _roleIds.Add(roleId);
            return this;
        }

        public AuthUser Build() {
            if (_id is null) {
                throw new InvalidOperationException("Id is not nullable");
            }
            if (_username is null) {
                throw new InvalidOperationException("Username is not nullable");
            }
            if (_passwordHash is null) {
                throw new InvalidOperationException("PasswordHash is not nullable");
            }
            if (_email is null) {
                throw new InvalidOperationException("Email is not nullable");
            }
            if (_createdAt is null) {
                throw new InvalidOperationException("CreatedAt is not nullable");
            }

            return new AuthUser(_id, _username, _passwordHash, _email, _createdAt.Value, _roleIds);
        }
    }
}
