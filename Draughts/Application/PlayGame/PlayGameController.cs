using Draughts.Application.Hubs;
using Draughts.Application.PlayGame.Services;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NodaTime;
using System.Threading.Tasks;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Application.PlayGame;

public sealed class PlayGameController : BaseController {
    private readonly IClock _clock;
    private readonly PlayGameService _playGameService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<WebsocketHub> _websocketHub;

    public PlayGameController(IClock clock, PlayGameService playGameService,
            IUnitOfWork unitOfWork, IHubContext<WebsocketHub> websocketHub) {
        _clock = clock;
        _playGameService = playGameService;
        _unitOfWork = unitOfWork;
        _websocketHub = websocketHub;
    }

    [HttpGet("/game/{gameId:long}"), GuestRoute]
    public IActionResult Game(long gameId) {
        try {
            var (game, gameState) = _unitOfWork.WithGameTransaction(tran => {
                return _playGameService.FindGameAndState(new GameId(gameId));
            });

            return View(new PlayGameViewModel(game, gameState, _clock, WebsiteContext.Nonce));
        } catch (ManualValidationException e) {
            return NotFound(e.Message);
        }
    }

    [HttpGet("/game/{gameId:long}/json"), GuestRoute]
    public IActionResult GameJson(long gameId) {
        try {
            var (game, gameState) = _unitOfWork.WithGameTransaction(tran => {
                return _playGameService.FindGameAndState(new GameId(gameId));
            });

            return Ok(new GameDto(game, gameState, _clock));
        } catch (ManualValidationException e) {
            return NotFound(e.Message);
        }
    }

    [HttpPost("/game/{gameId:long}/move"), Requires(Permissions.PLAY_GAME)]
    public async Task<IActionResult> Move(long gameId, [FromBody] MoveRequest? request) {
        try {
            ValidateNotNull(request?.From, request?.To);

            var (game, gameState) = _playGameService.DoMove(AuthContext.UserId, new GameId(gameId),
                new SquareId(request!.From), new SquareId(request.To));
            var data = new GameDto(game, gameState, _clock);

            await _websocketHub.PushGameUpdated(new GameId(gameId), data);

            return Ok(data);
        } catch (ManualValidationException e) {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("/game/{gameId:long}/draw"), Requires(Permissions.PLAY_GAME)]
    public async Task<IActionResult> Draw(long gameId) {
        try {
            _playGameService.VoteForDraw(AuthContext.UserId, new GameId(gameId));
            await _websocketHub.PushGameUpdateReady(new GameId(gameId));

            return SuccessRedirect($"/game/{gameId}", $"You've voted for a draw in game {gameId}");
        } catch (ManualValidationException e) {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("/game/{gameId:long}/resign"), Requires(Permissions.PLAY_GAME)]
    public async Task<IActionResult> Resign(long gameId) {
        try {
            _playGameService.Resign(AuthContext.UserId, new GameId(gameId));
            await _websocketHub.PushGameUpdateReady(new GameId(gameId));

            return SuccessRedirect($"/game/{gameId}", $"You've resigned from game {gameId}");
        } catch (ManualValidationException e) {
            return BadRequest(e.Message);
        }
    }

    public record MoveRequest(int? From, int? To);
}
