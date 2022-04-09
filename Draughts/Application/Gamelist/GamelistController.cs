using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common.OoConcepts;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.GameContext.Specifications;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Application;

[Requires(Permissions.PLAY_GAME)]
public sealed class GamelistController : BaseController {
    private const int PAGE_SIZE = 10;

    private readonly IClock _clock;
    private readonly IGameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GamelistController(IClock clock, IGameRepository gameRepository, IUnitOfWork unitOfWork) {
        _clock = clock;
        _gameRepository = gameRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult Pending(int page = 1) {
        var games = _unitOfWork.WithGameTransaction(tran => {
            return MyGameList(page, new PendingGameSpecification());
        });
        return View(new GamelistAndMenuViewModel(games, BuildMenu(), _clock));
    }

    [HttpGet]
    public IActionResult Active(int page = 1) {
        var games = _unitOfWork.WithGameTransaction(tran => {
            return MyGameList(page, new ActiveGameSpecification());
        });
        return View(new GamelistAndMenuViewModel(games, BuildMenu(), _clock));
    }

    [HttpGet]
    public IActionResult Finished(int page = 1) {
        var games = _unitOfWork.WithGameTransaction(tran => {
            return MyGameList(page, new FinishedGameSpecification());
        });
        return View(new GamelistAndMenuViewModel(games, BuildMenu(), _clock));
    }

    private Pagination<Game> MyGameList(int page, Specification<Game> statusSpec) {
        var gamelistSpec = statusSpec.And(new ContainsPlayerSpecification(AuthContext.UserId));
        return _gameRepository.Paginate(page, PAGE_SIZE, gamelistSpec, new GameIdSort());
    }

    private MenuViewModel BuildMenu() {
        return new MenuViewModel("Your games",
            ("Pending games", "/gamelist/pending"),
            ("Active games", "/gamelist/active"),
            ("Finished games", "/gamelist/finished"),
            ("Create a new game", "/lobby/create"));
    }
}
