using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class AuthUserId : IdValueObject<AuthUserId> {
        public override long Id { get; }

        public AuthUserId(long id) {
            if (id <= 0) {
                throw new ManualValidationException("Invalid user id.");
            }
            Id = id;
        }

        public static implicit operator long(AuthUserId authUserId) => authUserId.Id;
        public static implicit operator string(AuthUserId authUserId) => authUserId.ToString();
    }
}
