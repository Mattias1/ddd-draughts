using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Misc;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Application.Shared.ViewModels;

public class GamelistViewModel : IPaginationViewModel<GameViewModel> {
    public Pagination<GameViewModel> Pagination { get; }
    public IReadOnlyList<GameViewModel> Games => Pagination.Results;

    public GamelistViewModel(Pagination<Game> games, IClock clock) {
        Pagination = games.Map(u => new GameViewModel(u, clock));
    }
}
