using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameAggregate.Models {
    public class GameId : IdValueObject<GameId> {
        public override long Id { get; }

        public GameId(long? id) {
            if (id is null || id.Value <= 0) {
                throw new ManualValidationException("Invalid game id.");
            }
            Id = id.Value;
        }

        public static implicit operator long(GameId gameId) => gameId.Id;
        public static implicit operator string(GameId gameId) => gameId.ToString();
    }
}
