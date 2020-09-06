using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Controllers.Attributes;
using Draughts.Controllers.Shared.ViewModels;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.GameAggregate.Specifications;
using Draughts.Repositories;
using Draughts.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Controllers {
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

        [HttpGet("lobby/create"), Requires(Permissions.PLAY_GAME)]
        public IActionResult Create() {
            return View();
        }

        [HttpPost("lobby/create"), Requires(Permissions.PLAY_GAME)]
        public IActionResult PostCreate([FromForm] GameCreationRequest request) {
            try {
                var joinColor = ColorFromRequest(request.JoinAs);
                _gameService.CreateGame(AuthContext.UserId, request.BuildGameSettings(), joinColor);

                return Redirect("/lobby");
            }
            catch (ManualValidationException e) {
                Create();
                return ErrorRedirect("/lobby/create", e.Message);
            }
        }

        private Color ColorFromRequest(string? color) => color switch
        {
            "white" => Color.White,
            "black" => Color.Black,
            "random" => Rand.NextBool() ? Color.White : Color.Black,
            _ => throw new ManualValidationException("Unknown color choice.")
        };

        public class GameCreationRequest {
            public int BoardSize { get; set; }
            public bool WhiteHasFirstMove { get; set; }
            public bool FlyingKings { get; set; }
            public bool MenCaptureBackwards { get; set; }
            public string? CaptureConstraints { get; set; }
            public string? JoinAs { get; set; }

            public GameSettings BuildGameSettings() {
                var firstMove = WhiteHasFirstMove ? Color.White : Color.Black;
                var capConstraints = CaptureConstraints switch
                {
                    "max" => GameSettings.DraughtsCaptureConstraints.MaximumPieces,
                    "seq" => GameSettings.DraughtsCaptureConstraints.AnyFinishedSequence,
                    _ => throw new InvalidOperationException("Unknown capture constraint.")
                };

                return new GameSettings(BoardSize, firstMove, FlyingKings, MenCaptureBackwards, capConstraints);
            }
        }
    }
}
