using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameContext.Models {
    public class GameId : IdValueObject<GameId> {
        public override long Value { get; }

        public GameId(long? id) {
            if (id is null || id.Value <= 0) {
                throw new ManualValidationException("Invalid game id.");
            }
            Value = id.Value;
        }
    }
}
