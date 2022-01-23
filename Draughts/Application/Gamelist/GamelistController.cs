using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common.OoConcepts;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.GameContext.Specifications;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Application;

[Requires(Permissions.PLAY_GAME)]
public class GamelistController : BaseController {
    private const int PAGE_SIZE = 20;

    private readonly IGameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GamelistController(IGameRepository gameRepository, IUnitOfWork unitOfWork) {
        _gameRepository = gameRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult Pending(int page = 1) {
        var games = _unitOfWork.WithGameTransaction(tran => {
            return MyGameList(page, new PendingGameSpecification());
        });
        return View(new GamelistAndMenuViewModel(games, BuildMenu()));
    }

    [HttpGet]
    public IActionResult Active(int page = 1) {
        var games = _unitOfWork.WithGameTransaction(tran => {
            return MyGameList(page, new ActiveGameSpecification());
        });
        return View(new GamelistAndMenuViewModel(games, BuildMenu()));
    }

    [HttpGet]
    public IActionResult Finished(int page = 1) {
        var games = _unitOfWork.WithGameTransaction(tran => {
            return MyGameList(page, new FinishedGameSpecification());
        });
        return View(new GamelistAndMenuViewModel(games, BuildMenu()));
    }

    private Pagination<Game> MyGameList(int page, Specification<Game> statusSpec) {
        return _gameRepository.Paginate(
            page, PAGE_SIZE,
            statusSpec.And(new ContainsPlayerSpecification(AuthContext.UserId)),
            new GameIdSort()
        );
    }

    private MenuViewModel BuildMenu() {
        return new MenuViewModel("Your games",
            ("Pending games", "/gamelist/pending"),
            ("Active games", "/gamelist/active"),
            ("Finished games", "/gamelist/finished"),
            ("Create a new game", "/lobby/create")
        );
    }
}
