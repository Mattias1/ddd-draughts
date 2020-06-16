using System;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public readonly struct Permission {
        public string Value { get; }

        public Permission(string permissionname) => Value = permissionname.ToLower();

        public override bool Equals(object? obj) => obj is Permission permission && Equals(permission);
        public bool Equals(Permission other) => Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        public override int GetHashCode() => Value.GetHashCode();
        public static bool operator ==(Permission left, Permission right) => left.Equals(right);
        public static bool operator !=(Permission left, Permission right) => !left.Equals(right);

        public static class Permissions {
            public const string PLAY_GAME = "game.play";
            public const string VIEW_MOD_PANEL = "view.modpanel";
            public const string EDIT_ROLES = "role.edit";

            public static Permission PlayGame => new Permission(PLAY_GAME);
            public static Permission ViewModPanel => new Permission(VIEW_MOD_PANEL);
            public static Permission EditRoles => new Permission(EDIT_ROLES);
        }
    }
}
