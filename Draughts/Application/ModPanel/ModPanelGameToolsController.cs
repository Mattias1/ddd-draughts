using Draughts.Application.Auth.Services;
using Draughts.Application.Hubs;
using Draughts.Application.ModPanel.ViewModels;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.GameContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NodaTime;
using System.Threading.Tasks;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Application.ModPanel;

[ViewsFrom("ModPanel")]
public sealed class ModPanelGameToolsController : BaseController {
    private readonly AdminLogFactory _adminLogFactory;
    private readonly IClock _clock;
    private readonly GameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<WebsocketHub> _websocketHub;

    public ModPanelGameToolsController(AdminLogFactory adminLogFactory, IClock clock, GameRepository gameRepository,
            IUnitOfWork unitOfWork, IHubContext<WebsocketHub> websocketHub) {
        _adminLogFactory = adminLogFactory;
        _clock = clock;
        _gameRepository = gameRepository;
        _unitOfWork = unitOfWork;
        _websocketHub = websocketHub;
    }

    [HttpGet("/modpanel/game-tools"), Requires(Permissions.EDIT_GAMES)]
    public IActionResult GameTools() {
        return View(new ModPanelViewModel(ModPanelController.BuildMenu()));
    }

    [HttpPost("/modpanel/game-tools/turn-time"), Requires(Permissions.EDIT_GAMES)]
    public async Task<IActionResult> ChangeTurnTime([FromForm] ChangeTurnTimeRequest? request) {
        try {
            ValidateNotNull(request?.GameId, request?.TurnTimeInSeconds);

            var gameId = new GameId(request?.GameId);
            int turnTimeInSeconds = request!.TurnTimeInSeconds!.Value;
            bool forAllFutureTurns = request.ForAllFutureTurns ?? false;
            _adminLogFactory.LogChangeTurnTime(AuthContext.UserId, AuthContext.Username,
                gameId, turnTimeInSeconds, forAllFutureTurns);

            _unitOfWork.WithGameTransaction(tran => {
                var game = _gameRepository.FindById(gameId);
                game.ChangeTurnTime(_clock.UtcNow(), turnTimeInSeconds, forAllFutureTurns);
                _gameRepository.Save(game);
            });

            await _websocketHub.PushGameUpdateReady(gameId);

            return SuccessRedirect("/modpanel/game-tools", $"Turn time for game {gameId} is changed.");
        } catch (ManualValidationException e) {
            return ErrorRedirect("/modpanel/game-tools", e.Message);
        }
    }

    public record ChangeTurnTimeRequest(long? GameId, int? TurnTimeInSeconds, bool? ForAllFutureTurns);
}
