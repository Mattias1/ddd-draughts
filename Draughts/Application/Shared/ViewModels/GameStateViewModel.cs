using Draughts.Domain.GameContext.Models;
using System.Collections.Generic;

namespace Draughts.Application.Shared.ViewModels {
    public class GameStateViewModel {
        public GameId Id { get; }
        public Board Board { get; }
        public SquareId? CaptureSequenceFrom { get; private set; }
        public IReadOnlyList<Move> Moves;

        public GameStateViewModel(GameState gameState) {
            Id = gameState.Id;
            Board = gameState.Board;
            CaptureSequenceFrom = gameState.CaptureSequenceFrom;
            Moves = gameState.Moves;
        }
    }
}
