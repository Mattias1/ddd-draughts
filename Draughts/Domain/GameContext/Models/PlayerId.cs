using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameContext.Models;

public sealed class PlayerId : IdValueObject<PlayerId> {
    public override long Value { get; }

    public PlayerId(long id) {
        if (id <= 0) {
            throw new ManualValidationException("Invalid game id.");
        }
        Value = id;
    }
}
