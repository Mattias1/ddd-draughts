using Draughts.Application.Lobby.Services;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.GameContext.Specifications;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Application.Lobby;

public class LobbyController : BaseController {
    private readonly IGameRepository _gameRepository;
    private readonly GameService _gameService;
    private readonly IUnitOfWork _unitOfWork;

    public LobbyController(IGameRepository gameRepository, GameService gameService, IUnitOfWork unitOfWork) {
        _gameRepository = gameRepository;
        _gameService = gameService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("/lobby"), GuestRoute]
    public IActionResult Lobby() {
        var pendingGames = _unitOfWork.WithGameTransaction(tran => {
            return _gameRepository.List(new PendingGameSpecification());
        });
        return View(new GamelistViewModel(pendingGames));
    }

    [HttpGet("/lobby/spectate"), GuestRoute]
    public IActionResult Spectate() {
        var activeGames = _unitOfWork.WithGameTransaction(tran => {
            return _gameRepository.List(new ActiveGameSpecification());
        });
        return View(new GamelistViewModel(activeGames));
    }

    [HttpGet("lobby/create"), Requires(Permissions.PLAY_GAME)]
    public IActionResult Create() {
        return View();
    }

    [HttpPost("lobby/create"), Requires(Permissions.PLAY_GAME)]
    public IActionResult PostCreate([FromForm] GameCreationRequest? request) {
        try {
            ValidateNotNull(request?.BoardSize, request?.WhiteHasFirstMove, request?.FlyingKings,
                request?.MenCaptureBackwards, request?.CaptureConstraints, request?.JoinAs);

            var joinColor = ColorFromRequest(request!.JoinAs);
            var game = _gameService.CreateGame(AuthContext.UserId, request!.BuildGameSettings(), joinColor);

            return SuccessRedirect($"/game/{game.Id}", $"Game {game.Id} is created.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect("/lobby/create", e.Message);
        }
    }

    [HttpPost("lobby/join"), Requires(Permissions.PLAY_GAME)]
    public IActionResult PostJoin([FromForm] GameJoinRequest? request) {
        try {
            ValidateNotNull(request?.GameId);

            var joinColor = request!.Color is null ? null : ColorFromRequest(request.Color);
            _gameService.JoinGame(AuthContext.UserId, new GameId(request.GameId), joinColor);

            return SuccessRedirect("/game/" + request.GameId, $"You've joined game {request.GameId}.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect("/lobby", e.Message);
        }
    }

    private Color ColorFromRequest(string? color) => color switch {
        "white" => Color.White,
        "black" => Color.Black,
        "random" => Color.Random,
        _ => throw new ManualValidationException("Unknown color choice.")
    };

    public record GameCreationRequest(int? BoardSize, bool? WhiteHasFirstMove,
            bool? FlyingKings, bool? MenCaptureBackwards, string? CaptureConstraints, string? JoinAs) {
        public GameSettings BuildGameSettings() {
            var firstMove = WhiteHasFirstMove!.Value ? Color.White : Color.Black;
            var capConstraints = CaptureConstraints switch {
                "max" => GameSettings.DraughtsCaptureConstraints.MaximumPieces,
                "seq" => GameSettings.DraughtsCaptureConstraints.AnyFinishedSequence,
                _ => throw new ManualValidationException("Unknown capture constraint.")
            };

            return new GameSettings(BoardSize!.Value, firstMove, FlyingKings!.Value, MenCaptureBackwards!.Value, capConstraints);
        }
    }

    public record GameJoinRequest(long? GameId, string? Color);
}
