using Draughts.Domain.GameContext.Models;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.Shared.ViewModels;

public class GamelistViewModel {
    public IReadOnlyList<GameViewModel> Games { get; set; }

    public GamelistViewModel(IReadOnlyList<Game> games, IClock clock) {
        Games = games.Select(u => new GameViewModel(u, clock)).ToList().AsReadOnly();
    }
}
