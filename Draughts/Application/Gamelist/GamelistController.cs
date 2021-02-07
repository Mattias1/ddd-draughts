using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common.OoConcepts;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.GameAggregate.Specifications;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Application {
    [Requires(Permissions.PLAY_GAME)]
    public class GamelistController : BaseController {
        private readonly IGameRepository _gameRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GamelistController(IGameRepository gameRepository, IUnitOfWork unitOfWork) {
            _gameRepository = gameRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Pending() {
            var games = _unitOfWork.WithGameTransaction(tran => {
                var games = MyGameList(new PendingGameSpecification());
                return tran.CommitWith(games);
            });
            return View(new GamelistAndMenuViewModel(games, BuildMenu()));
        }

        [HttpGet]
        public IActionResult Active() {
            var games = _unitOfWork.WithGameTransaction(tran => {
                var games = MyGameList(new ActiveGameSpecification());
                return tran.CommitWith(games);
            });
            return View(new GamelistAndMenuViewModel(games, BuildMenu()));
        }

        [HttpGet]
        public IActionResult Finished() {
            var games = _unitOfWork.WithGameTransaction(tran => {
                var games = MyGameList(new FinishedGameSpecification());
                return tran.CommitWith(games);
            });
            return View(new GamelistAndMenuViewModel(games, BuildMenu()));
        }

        private IReadOnlyList<Game> MyGameList(Specification<Game> statusSpec) {
            return _gameRepository.List(statusSpec.And(new ContainsPlayerSpecification(AuthContext.UserId)));
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
