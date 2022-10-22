using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Misc;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Application.Shared.ViewModels;

public sealed class GamelistAndMenuViewModel : IPaginationViewModel<GameViewModel> {
    public Pagination<GameViewModel> Pagination { get; }
    public IReadOnlyList<GameViewModel> Games => Pagination.Results;
    public MenuViewModel Menu { get; }

    public GamelistAndMenuViewModel(Pagination<Game> games, MenuViewModel menuViewModel, IClock clock) {
        Pagination = games.Map(u => new GameViewModel(u, clock));
        Menu = menuViewModel;
    }
}
