using Draughts.Application.PlayGame.Services;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Application.PlayGame {
    public class PlayGameController : BaseController {
        private readonly IGameRepository _gameRepository;
        private readonly IPlayGameService _playGameService;
        private readonly IUnitOfWork _unitOfWork;

        public PlayGameController(IGameRepository gameRepository, IPlayGameService playGameService, IUnitOfWork unitOfWork) {
            _gameRepository = gameRepository;
            _playGameService = playGameService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("/game/{gameId:long}"), GuestRoute]
        public IActionResult Game(long gameId) {
            var game = _unitOfWork.WithGameTransaction(tran => {
                var game = _gameRepository.FindByIdOrNull(new GameId(gameId));
                return tran.CommitWith(game);
            });

            if (game is null) {
                return NotFound();
            }

            return View(new GameViewModel(game));
        }

        [HttpPost("/game/{gameId:long}/move"), Requires(Permissions.PLAY_GAME)]
        public IActionResult Move(long gameId, [FromBody] MoveRequest? request) {
            try {
                ValidateNotNull(request?.From, request?.To);

                _playGameService.DoMove(AuthContext.UserId, new GameId(gameId),
                    new SquareId(request!.From), new SquareId(request.To));

                return Ok();
            }
            catch (ManualValidationException e) {
                return BadRequest(e.Message);
            }
        }

        public class MoveRequest {
            public int? From { get; set; }
            public int? To { get; set; }
        }
    }
}
