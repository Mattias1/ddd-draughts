using Draughts.Domain.GameContext.Models;
using System.Collections.Generic;

namespace Draughts.Application.Shared.ViewModels;

public class GameStateViewModel {
    public GameId Id { get; }
    public BoardViewModel Board { get; }
    public SquareId? CaptureSequenceFrom { get; private set; }
    public IReadOnlyList<Move> Moves;

    public GameStateViewModel(GameState gameState) {
        Id = gameState.Id;
        Board = new BoardViewModel(gameState.Board);
        CaptureSequenceFrom = gameState.CaptureSequenceFrom;
        Moves = gameState.Moves;
    }
}
