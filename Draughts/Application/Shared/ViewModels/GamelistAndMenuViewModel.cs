using Draughts.Domain.GameAggregate.Models;
using Draughts.Repositories;
using System.Collections.Generic;

namespace Draughts.Application.Shared.ViewModels {
    public class GamelistAndMenuViewModel : IPaginationViewModel<GameViewModel> {
        public Pagination<GameViewModel> Pagination { get; }
        public IReadOnlyList<GameViewModel> Games => Pagination.Results;
        public MenuViewModel Menu { get; }

        public GamelistAndMenuViewModel(Pagination<Game> games, MenuViewModel menuViewModel) {
            Pagination = games.Map(u => new GameViewModel(u));
            Menu = menuViewModel;
        }
    }
}
