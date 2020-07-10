using Draughts.Common;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class Permission : StringValueObject<Permission> {
        public override string Value { get; }

        public Permission(string permissionname) => Value = permissionname.ToLower();

        public static class Permissions {
            public const string PENDING_REGISTRATION = "role.pendingregistration";
            public const string PLAY_GAME = "game.play";
            public const string VIEW_MOD_PANEL = "view.modpanel";
            public const string EDIT_ROLES = "role.edit";

            public static Permission PendingRegistration => new Permission(PENDING_REGISTRATION);
            public static Permission PlayGame => new Permission(PLAY_GAME);
            public static Permission ViewModPanel => new Permission(VIEW_MOD_PANEL);
            public static Permission EditRoles => new Permission(EDIT_ROLES);
        }
    }
}
