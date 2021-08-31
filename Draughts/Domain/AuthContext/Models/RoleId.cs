using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.AuthContext.Models {
    public class RoleId : IdValueObject<RoleId> {
        public override long Value { get; }

        public RoleId(long id) {
            if (id <= 0) {
                throw new ManualValidationException("Invalid role id.");
            }
            Value = id;
        }
    }
}
