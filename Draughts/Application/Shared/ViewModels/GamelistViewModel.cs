using Draughts.Domain.GameAggregate.Models;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.Shared.ViewModels {
    public class GamelistViewModel {
        public IReadOnlyList<GameViewModel> Games { get; set; }

        public GamelistViewModel(IReadOnlyList<Game> games) {
            Games = games.Select(u => new GameViewModel(u)).ToList().AsReadOnly();
        }
    }
}
