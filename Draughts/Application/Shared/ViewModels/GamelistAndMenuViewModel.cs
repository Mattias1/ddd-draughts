using Draughts.Domain.GameAggregate.Models;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.Shared.ViewModels {
    public class GamelistAndMenuViewModel {
        public IReadOnlyList<GameViewModel> Games { get; set; }
        public MenuViewModel Menu { get; set; }

        public GamelistAndMenuViewModel(IReadOnlyList<Game> games, MenuViewModel menuViewModel) {
            Games = games.Select(u => new GameViewModel(u)).ToList().AsReadOnly();
            Menu = menuViewModel;
        }
    }
}
