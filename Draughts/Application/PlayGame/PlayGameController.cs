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
        private readonly IPlayGameService _playGameService;
        private readonly IUnitOfWork _unitOfWork;

        public PlayGameController(IPlayGameService playGameService, IUnitOfWork unitOfWork) {
            _playGameService = playGameService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("/game/{gameId:long}"), GuestRoute]
        public IActionResult Game(long gameId) {
            try {
                var (game, gameState) = _unitOfWork.WithGameTransaction(tran => {
                    var gameAndStatePair = _playGameService.FindGameAndState(new GameId(gameId));
                    return tran.CommitWith(gameAndStatePair);
                });

                return View(new PlayGameViewModel(game, gameState));
            }
            catch (ManualValidationException e) {
                return NotFound(e.Message);
            }
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

        public record MoveRequest(int? From, int? To);
    }
}
