using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Draughts.Application.PlayGame {
    public class PlayGameController : BaseController {
        private readonly IGameRepository _gameRepository;

        public PlayGameController(IGameRepository gameRepository) {
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
