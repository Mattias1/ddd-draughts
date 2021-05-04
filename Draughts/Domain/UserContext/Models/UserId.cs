using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.UserContext.Models {
    public class UserId : IdValueObject<UserId> {
        public override long Id { get; }

        public UserId(long id) {
            if (id <= 0) {
                throw new ManualValidationException("Invalid user id.");
            }
            Id = id;
        }

        public static implicit operator long(UserId? userId) => userId?.Id ?? 0;
        public static implicit operator string(UserId? userId) => userId?.ToString() ?? "";
    }
}
