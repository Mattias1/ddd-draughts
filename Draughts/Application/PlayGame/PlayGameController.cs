using Draughts.Application.PlayGame.Services;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Application.PlayGame {
    public class PlayGameController : BaseController {
        private readonly PlayGameService _playGameService;
        private readonly IUnitOfWork _unitOfWork;

        public PlayGameController(PlayGameService playGameService, IUnitOfWork unitOfWork) {
            _playGameService = playGameService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("/game/{gameId:long}"), GuestRoute]
        public IActionResult Game(long gameId) {
            try {
                var (game, gameState) = _unitOfWork.WithGameTransaction(tran => {
                    return _playGameService.FindGameAndState(new GameId(gameId));
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

        [HttpPost("/game/{gameId:long}/draw"), Requires(Permissions.PLAY_GAME)]
        public IActionResult Draw(long gameId) {
            try {
                _playGameService.VoteForDraw(AuthContext.UserId, new GameId(gameId));
                return SuccessRedirect($"/game/{gameId}", $"You've voted for a draw in game {gameId}");
            }
            catch (ManualValidationException e) {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("/game/{gameId:long}/resign"), Requires(Permissions.PLAY_GAME)]
        public IActionResult Resign(long gameId) {
            try {
                _playGameService.Resign(AuthContext.UserId, new GameId(gameId));
                return SuccessRedirect($"/game/{gameId}", $"You've resigned from game {gameId}");
            }
            catch (ManualValidationException e) {
                return BadRequest(e.Message);
            }
        }

        public record MoveRequest(int? From, int? To);
    }
}
