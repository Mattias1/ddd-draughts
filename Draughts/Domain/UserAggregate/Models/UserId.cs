using Draughts.Common;

namespace Draughts.Domain.UserAggregate.Models {
    public class UserId : IdValueObject<UserId> {
        public override long Id { get; }

        public UserId(long id) {
            if (id <= 0) {
                throw new ManualValidationException("Invalid user id.");
            }
            Id = id;
        }

        public static implicit operator long(UserId userId) => userId.Id;
        public static implicit operator string(UserId userId) => userId.ToString();
    }
}
