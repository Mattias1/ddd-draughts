using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.AuthContext.Models {
    public class AdminLogId : IdValueObject<AdminLogId> {
        public override long Id { get; }

        public AdminLogId(long id) {
            if (id <= 0) {
                throw new ManualValidationException("Invalid admin log id.");
            }
            Id = id;
        }

        public static implicit operator long(AdminLogId? roleId) => roleId?.Id ?? 0;
        public static implicit operator string(AdminLogId? roleId) => roleId?.ToString() ?? "";
    }
}
