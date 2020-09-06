using Draughts.Common;
using Draughts.Controllers.Attributes;
using Draughts.Controllers.Shared.ViewModels;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Draughts.Controllers {
    public class GameController : BaseController {
        private readonly IGameRepository _gameRepository;

        public GameController(IGameRepository gameRepository) {
            _gameRepository = gameRepository;
        }

        [HttpGet("/game/{gameId:long}"), GuestRoute]
        public IActionResult Game(long gameId) {
            var game = _gameRepository.FindByIdOrNull(new GameId(gameId));

            if (game is null) {
                return NotFound();
            }

            return View(new GameViewModel(game));
        }
    }
}
