using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.UserContext.Models;

public sealed class UserId : IdValueObject<UserId> {
    public const long ADMIN = 1;

    public override long Value { get; }

    public UserId(long id) {
        if (id <= 0) {
            throw new ManualValidationException("Invalid user id.");
        }
        Value = id;
    }

    public static UserId? FromNullable(long? userId) => userId is null ? null : new UserId(userId.Value);
}
