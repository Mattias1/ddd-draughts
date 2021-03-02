using Draughts.Common.OoConcepts;
using System.Collections.Generic;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class Permission : StringValueObject<Permission> {
        public override string Value { get; }

        public Permission(string permissionname) => Value = permissionname.ToLower();

        public static class Permissions {
            public const string PENDING_REGISTRATION = "misc.pendingregistration";
            public const string PLAY_GAME = "game.play";
            public const string VIEW_MOD_PANEL = "modpanel.view";
            public const string VIEW_ADMIN_LOGS = "modpanel.adminlogs.view";
            public const string EDIT_GAMES = "modpanel.games.edit";
            public const string EDIT_ROLES = "modpanel.roles.edit";

            public static Permission PendingRegistration => new Permission(PENDING_REGISTRATION);
            public static Permission PlayGame => new Permission(PLAY_GAME);
            public static Permission ViewModPanel => new Permission(VIEW_MOD_PANEL);
            public static Permission ViewAdminLogs => new Permission(VIEW_ADMIN_LOGS);
            public static Permission EditGames => new Permission(EDIT_GAMES);
            public static Permission EditRoles => new Permission(EDIT_ROLES);

            public static IReadOnlyList<Permission> All => new List<Permission>() {
                PendingRegistration, PlayGame, ViewModPanel, ViewAdminLogs, EditGames, EditRoles
            }.AsReadOnly();
        }
    }
}
