using Draughts.Common.Utilities;
using Draughts.Domain.GameContext.Models;

namespace Draughts.Application.Shared.ViewModels {
    public class GameDto {
        public long Id { get; }
        public TurnDto? Turn { get; }
        public string GameEndedMessage { get; }
        public int? CaptureSequenceFrom { get; }
        public string BoardString { get; }

        // The empty constructor is here to create a DTO from JSON deserialisation.
        public GameDto() {
            Id = 0L;
            Turn = null;
            GameEndedMessage = "";
            CaptureSequenceFrom = null;
            BoardString = "";
        }
        public GameDto(Game game, GameState gameState) {
            Id = game.Id.Value;
            Turn = game.Turn is null ? null : new TurnDto(game.Turn);
            GameEndedMessage = BuildGameEndedMessage(game);
            CaptureSequenceFrom = gameState.CaptureSequenceFrom?.Value;
            BoardString = gameState.Board.ToString();
        }

        private string BuildGameEndedMessage(Game game) {
            if (!game.IsFinished) {
                return "";
            }
            if (game.Victor is null) {
                return "The game ended in a draw.";
            }
            return $"{game.Victor.Username} won the game.";
        }

        public class TurnDto {
            public long PlayerId { get; }
            public string ExpiresAt { get; }

            // The empty constructor is here to create a DTO from JSON deserialisation.
            public TurnDto() {
                PlayerId = 0L;
                ExpiresAt = "";
            }
            public TurnDto(Turn turn) {
                PlayerId = turn.Player.Id.Value;
                ExpiresAt = turn.ExpiresAt.ToIsoString();
            }
        }
    }
}
