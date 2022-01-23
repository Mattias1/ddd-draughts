using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.AuthContext.Models;

public class AdminLogId : IdValueObject<AdminLogId> {
    public override long Value { get; }

    public AdminLogId(long id) {
        if (id <= 0) {
            throw new ManualValidationException("Invalid admin log id.");
        }
        Value = id;
    }
}
