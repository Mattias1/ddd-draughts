using Draughts.Common.OoConcepts;
using Draughts.Common.Utilities;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using NodaTime;
using SqlQueryBuilder.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Domain.AuthContext.Models {
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
            string userId, username = "";
            switch (Type) {
                case "role.create":
                    var (roleId, rolename) = Parameters.UnpackDuo();
                    return $"Created a new role '{roleId} - {rolename}'";
                case "role.edit":
                    (roleId, rolename) = Parameters.UnpackDuo();
                    return $"Edited the role '{roleId} - {rolename}'";
                case "role.gained":
                    (roleId, rolename, userId, username) = Parameters.UnpackQuad();
                    return $"Assigned the role '{roleId} - {rolename}' to '{userId} - {username}'";
                case "role.lost":
                    (roleId, rolename, userId, username) = Parameters.UnpackQuad();
                    return $"Removed the role '{roleId} - {rolename}' from '{userId} - {username}'";
                default:
                    throw new InvalidOperationException("Unknown admin log type.");
            }
        }

        public static AdminLog CreateRoleLog(IIdPool idPool, IClock clock, AuthUser actor, RoleId roleId, string rolename) {
            return new AdminLog(
                Next(idPool), "role.create", Params(roleId, rolename),
                actor.Id, actor.Username,
                Permissions.EditRoles, clock.UtcNow()
            );
        }

        public static AdminLog EditRoleLog(IIdPool idPool, IClock clock, AuthUser actor, RoleId roleId, string rolename) {
            return new AdminLog(
                Next(idPool), "role.edit", Params(roleId, rolename),
                actor.Id, actor.Username,
                Permissions.EditRoles, clock.UtcNow()
            );
        }

        public static AdminLog RoleGainedLog(IIdPool idPool, IClock clock, AuthUser actor,
                RoleId roleId, string rolename, UserId userId, Username username) {
            return new AdminLog(
                Next(idPool), "role.gained", Params(roleId, rolename, userId, username),
                actor.Id, actor.Username,
                Permissions.EditRoles, clock.UtcNow()
            );
        }

        public static AdminLog RoleLostLog(IIdPool idPool, IClock clock, AuthUser actor,
                RoleId roleId, string rolename, UserId userId, Username username) {
            return new AdminLog(
                Next(idPool), "role.lost", Params(roleId, rolename, userId, username),
                actor.Id, actor.Username,
                Permissions.EditRoles, clock.UtcNow()
            );
        }

        private static AdminLogId Next(IIdPool idPool) => new AdminLogId(idPool.Next());
        private static IReadOnlyList<string> Params(params string[] parameters) => parameters.ToList().AsReadOnly();
    }
}
