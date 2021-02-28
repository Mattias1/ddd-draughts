using Draughts.Common.OoConcepts;
using Draughts.Common.Utilities;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class AdminLog : Entity<AdminLog, AdminLogId> {
        public override AdminLogId Id { get; }
        public string Type { get; }
        public IReadOnlyList<string> Parameters { get; }
        public UserId UserId { get; }
        public Username Username { get; }
        public Permission Permission { get; }
        public ZonedDateTime CreatedAt { get; }

        public AdminLog(AdminLogId id, string type, IReadOnlyList<string> parameters,
                UserId userId, Username username, Permission permission, ZonedDateTime createdAt) {
            Id = id;
            UserId = userId;
            Username = username;
            Type = type;
            Parameters = parameters;
            Permission = permission;
            CreatedAt = createdAt;
        }

        public string Description() {
            switch (Type) {
                case "role.create":
                    var (roleId, roleName) = UnpackTwoParameters();
                    return $"Created a new role '{roleId} - {roleName}'";
                case "role.edit":
                    (roleId, roleName) = UnpackTwoParameters();
                    return $"Edited the role '{roleId} - {roleName}'";
                default:
                    throw new InvalidOperationException("Unknown admin log type.");
            }
        }

        private (string, string) UnpackTwoParameters() {
            if (Parameters.Count != 2) {
                throw new InvalidOperationException("Invalid parameter unpacking.");
            }
            return (Parameters[0], Parameters[1]);
        }

        public static AdminLog CreateRoleLog(IIdPool idPool, IClock clock, AuthUser user, RoleId roleId, string rolename) {
            return new AdminLog(
                new AdminLogId(idPool.Next()), "role.create", Params(roleId, rolename),
                user.Id, user.Username,
                Permissions.EditRoles, clock.UtcNow()
            );
        }

        public static AdminLog EditRoleLog(IIdPool idPool, IClock clock, AuthUser user, RoleId roleId, string rolename) {
            return new AdminLog(
                new AdminLogId(idPool.Next()), "role.edit", Params(roleId, rolename),
                user.Id, user.Username,
                Permissions.EditRoles, clock.UtcNow()
            );
        }

        private static IReadOnlyList<string> Params(params string[] parameters) => parameters.ToList().AsReadOnly();
    }
}
