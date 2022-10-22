using Draughts.Common.Utilities;
using Draughts.Domain.GameContext.Models;
using NodaTime;

namespace Draughts.Application.Shared.ViewModels;

public sealed class GameDto {
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
    public GameDto(Game game, GameState gameState, IClock clock) {
        Id = game.Id.Value;
        Turn = game.Turn is null ? null : new TurnDto(game.Turn, clock);
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

    public sealed class TurnDto {
        public long PlayerId { get; }
        public string CurrentTime { get; }
        public string ExpiresAt { get; }

        // The empty constructor is here to create a DTO from JSON deserialisation.
        public TurnDto() {
            PlayerId = 0L;
            CurrentTime = "";
            ExpiresAt = "";
        }
        public TurnDto(Turn turn, IClock clock) {
            PlayerId = turn.Player.Id.Value;
            CurrentTime = clock.UtcNow().ToIsoString();
            ExpiresAt = turn.ExpiresAt.ToIsoString();
        }
    }
}
