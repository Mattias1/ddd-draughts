using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.UserContext.Models;

public class UserId : IdValueObject<UserId> {
    public override long Value { get; }

    public UserId(long id) {
        if (id <= 0) {
            throw new ManualValidationException("Invalid user id.");
        }
        Value = id;
    }
}
