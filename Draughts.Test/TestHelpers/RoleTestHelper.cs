using Draughts.Domain.AuthUserAggregate.Models;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Test.TestHelpers {
    public class RoleTestHelper {
        public static RoleBuilder PendingRegistration() {
            return new RoleBuilder()
                .WithId(IdTestHelper.Next())
                .WithRolename(Role.PENDING_REGISTRATION_ROLENAME)
                .WithPermissions(Permissions.PendingRegistration);
        }

        public static RoleBuilder RegisteredUser() {
            return new RoleBuilder()
                .WithId(IdTestHelper.Next())
                .WithRolename(Role.REGISTERED_USER_ROLENAME)
                .WithPermissions(Permissions.PlayGame);
        }


        public class RoleBuilder {
            private RoleId _id;
            private string _rolename = "";
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

            public RoleBuilder WithPermissions(params Permission[] permissions) => WithPermissions(permissions.ToList());
            public RoleBuilder WithPermissions(List<Permission> permissions) {
                _permissions = permissions;
                return this;
            }

            public Role Build() {
                return new Role(_id, _rolename, _permissions.ToArray());
            }
        }
    }
}
