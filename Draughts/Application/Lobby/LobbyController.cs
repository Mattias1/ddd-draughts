using Draughts.Application.Lobby.Services;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.GameAggregate.Specifications;
using Draughts.Repositories;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Application.Lobby {
    public class LobbyController : BaseController {
        private readonly IGameRepository _gameRepository;
        private readonly IGameService _gameService;

        public LobbyController(IGameRepository gameRepository, IGameService gameService) {
            _gameRepository = gameRepository;
            _gameService = gameService;
        }

        [HttpGet("/lobby"), GuestRoute]
        public IActionResult Lobby() {
            var pendingGames = _gameRepository.List(new PendingGameSpecification());
            return View(new GamelistViewModel(pendingGames));
        }

        [HttpGet("/lobby/spectate"), GuestRoute]
        public IActionResult Spectate() {
            var pendingGames = _gameRepository.List(new ActiveGameSpecification());
            return View(new GamelistViewModel(pendingGames));
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

                return Redirect($"/game/{game.Id}");
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

                return Redirect("/game/" + request.GameId);
            }
            catch (ManualValidationException e) {
                return ErrorRedirect("/lobby", e.Message);
            }
        }

        private Color ColorFromRequest(string? color) => color switch
        {
            "white" => Color.White,
            "black" => Color.Black,
            "random" => Color.Random,
            _ => throw new ManualValidationException("Unknown color choice.")
        };

        public class GameCreationRequest {
            public int? BoardSize { get; set; }
            public bool? WhiteHasFirstMove { get; set; }
            public bool? FlyingKings { get; set; }
            public bool? MenCaptureBackwards { get; set; }
            public string? CaptureConstraints { get; set; }
            public string? JoinAs { get; set; }

            public GameSettings BuildGameSettings() {
                var firstMove = WhiteHasFirstMove!.Value ? Color.White : Color.Black;
                var capConstraints = CaptureConstraints switch
                {
                    "max" => GameSettings.DraughtsCaptureConstraints.MaximumPieces,
                    "seq" => GameSettings.DraughtsCaptureConstraints.AnyFinishedSequence,
                    _ => throw new ManualValidationException("Unknown capture constraint.")
                };

                return new GameSettings(BoardSize!.Value, firstMove, FlyingKings!.Value, MenCaptureBackwards!.Value, capConstraints);
            }
        }

        public class GameJoinRequest {
            public long? GameId { get; set; }
            public string? Color { get; set; }
        }
    }
}
