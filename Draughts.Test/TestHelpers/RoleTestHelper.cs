using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Models;
using NodaTime;
using NodaTime.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Test.TestHelpers {
    public class RoleTestHelper {
        private static readonly ZonedDateTime Feb29 = FakeClock.FromUtc(2020, 02, 29).UtcNow();

        public static RoleBuilder PendingRegistration() {
            return new RoleBuilder()
                .WithId(IdTestHelper.Next())
                .WithRolename(Role.PENDING_REGISTRATION_ROLENAME)
                .WithCreatedAt(Feb29)
                .WithPermissions(Permissions.PendingRegistration);
        }

        public static RoleBuilder RegisteredUser() {
            return new RoleBuilder()
                .WithId(IdTestHelper.Next())
                .WithRolename(Role.REGISTERED_USER_ROLENAME)
                .WithCreatedAt(Feb29)
                .WithPermissions(Permissions.PlayGame);
        }

        public static RoleBuilder Admin() {
            return new RoleBuilder()
                .WithId(IdTestHelper.Next())
                .WithRolename(Role.ADMIN_ROLENAME)
                .WithCreatedAt(Feb29)
                .WithPermissions(Permissions.PlayGame,
                    Permissions.ViewModPanel, Permissions.EditGames,
                    Permissions.EditRoles, Permissions.ViewAdminLogs);
        }


        public class RoleBuilder {
            private RoleId? _id;
            private string? _rolename;
            private ZonedDateTime? _createdAt;
            private List<Permission> _permissions = new List<Permission>();

            public RoleBuilder WithId(long id) => WithId(new RoleId(id));
            public RoleBuilder WithId(RoleId id) {
                _id = id;
                return this;
            }

            public RoleBuilder WithRolename(string rolename) {
                _rolename = rolename;
                return this;
            }

            public RoleBuilder WithCreatedAt(ZonedDateTime createdAt) {
                _createdAt = createdAt;
                return this;
            }

            public RoleBuilder WithPermissions(params Permission[] permissions) => WithPermissions(permissions.ToList());
            public RoleBuilder WithPermissions(List<Permission> permissions) {
                _permissions = permissions;
                return this;
            }

            public Role Build() {
                if (_id is null) {
                    throw new InvalidOperationException("Id is not nullable");
                }
                if (_rolename is null) {
                    throw new InvalidOperationException("Rolename is not nullable");
                }
                if (_createdAt is null) {
                    throw new InvalidOperationException("CreatedAt is not nullable");
                }

                return new Role(_id, _rolename, _createdAt.Value, _permissions.ToArray());
            }
        }
    }
}
