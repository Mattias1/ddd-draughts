using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.AuthUserContext.Models {
    public class RoleId : IdValueObject<RoleId> {
        public override long Id { get; }

        public RoleId(long id) {
            if (id <= 0) {
                throw new ManualValidationException("Invalid role id.");
            }
            Id = id;
        }

        public static implicit operator long(RoleId roleId) => roleId.Id;
        public static implicit operator string(RoleId roleId) => roleId.ToString();
    }
}
