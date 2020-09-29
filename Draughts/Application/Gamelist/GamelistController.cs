using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.GameAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Application {
    [Requires(Permissions.PLAY_GAME)]
    public class GamelistController : BaseController {
        private readonly IGameRepository _gameRepository;

        public GamelistController(IGameRepository gameRepository) {
            _gameRepository = gameRepository;
        }

        [HttpGet]
        public IActionResult Pending() {
            var games = _gameRepository.List(new PendingGameSpecification().And(new ContainsPlayerSpecification(AuthContext.UserId)));
            return View(new GamelistAndMenuViewModel(games, BuildMenu()));
        }

        [HttpGet]
        public IActionResult Active() {
            var games = _gameRepository.List(new ActiveGameSpecification().And(new ContainsPlayerSpecification(AuthContext.UserId)));
            return View(new GamelistAndMenuViewModel(games, BuildMenu()));
        }

        [HttpGet]
        public IActionResult Finished() {
            var games = _gameRepository.List(new FinishedGameSpecification().And(new ContainsPlayerSpecification(AuthContext.UserId)));
            return View(new GamelistAndMenuViewModel(games, BuildMenu()));
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
}
