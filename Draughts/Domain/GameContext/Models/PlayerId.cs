using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameContext.Models {
    public class PlayerId : IdValueObject<PlayerId> {
        public override long Id { get; }

        public PlayerId(long id) {
            if (id <= 0) {
                throw new ManualValidationException("Invalid game id.");
            }
            Id = id;
        }

        public static implicit operator long(PlayerId? playerId) => playerId?.Id ?? 0;
        public static implicit operator string(PlayerId? playerId) => playerId?.ToString() ?? "";
    }
}
