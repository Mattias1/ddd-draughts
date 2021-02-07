using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using NodaTime;
using NodaTime.Testing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Test.TestHelpers {
    public class AuthUserTestHelper {
        private static readonly ZonedDateTime Feb29 = FakeClock.FromUtc(2020, 02, 29).UtcNow();

        public static AuthUserBuilder FromUser(User user) {
            var registeredUserRole = RoleTestHelper.RegisteredUser().Build();
            return FromUserAndRoles(user, registeredUserRole);
        }

        public static AuthUserBuilder FromUserAndRoles(User user, params Role[] roles) {
            return new AuthUserBuilder()
                .WithId(user.AuthUserId)
                .WithUserId(user.Id)
                .WithUsername(user.Username)
                .WithEmail($"{user.Username}@example.com")
                .WithPasswordHash(user.Username)
                .WithCreatedAt(user.CreatedAt)
                .WithRoles(roles);
        }

        public static AuthUserBuilder User(string name = "user") {
            var authUserId = new AuthUserId(IdTestHelper.Next());
            var username = new Username(name);
            var registeredUserRole = RoleTestHelper.RegisteredUser().Build();

            return new AuthUserBuilder()
                .WithId(authUserId)
                .WithUserId(IdTestHelper.Next())
                .WithUsername(username)
                .WithEmail($"{name}@example.com")
                .WithPasswordHash(name)
                .WithCreatedAt(Feb29)
                .WithRoles(registeredUserRole);
        }


        public class AuthUserBuilder {
            private AuthUserId? _id;
            private UserId? _userId;
            private Username? _username;
            private PasswordHash? _passwordHash;
            private Email? _email;
            private ZonedDateTime? _createdAt;
            private List<Role> _roles = new List<Role>();

            public AuthUserBuilder WithId(long id) => WithId(new AuthUserId(id));
            public AuthUserBuilder WithId(AuthUserId id) {
                _id = id;
                return this;
            }

            public AuthUserBuilder WithUserId(long userId) => WithUserId(new UserId(userId));
            public AuthUserBuilder WithUserId(UserId userId) {
                _userId = userId;
                return this;
            }

            public AuthUserBuilder WithUsername(string username) => WithUsername(new Username(username));
            public AuthUserBuilder WithUsername(Username username) {
                _username = username;
                return this;
            }

            public AuthUserBuilder WithPasswordHash(string plaintextPassword) {
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

            public AuthUserBuilder WithRoles(params Role[] roles) => WithRoles(roles.ToList());
            public AuthUserBuilder WithRoles(List<Role> roles) {
                _roles = roles;
                return this;
            }

            public AuthUser Build() {
                if (_id is null) {
                    throw new InvalidOperationException("Id is not nullable");
                }
                if (_userId is null) {
                    throw new InvalidOperationException("UserId is not nullable");
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

                return new AuthUser(_id, _userId, _username, _passwordHash, _email, _createdAt.Value, _roles);
            }
        }
    }
}
