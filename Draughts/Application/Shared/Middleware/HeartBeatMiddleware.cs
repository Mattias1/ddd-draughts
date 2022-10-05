using DalSoft.Hosting.BackgroundQueue;
using Draughts.Domain.GameContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NodaTime;
using SignalRWebPack.Hubs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Draughts.Application.Shared.Middleware;

public sealed class HeartBeatMiddleware {
    private const int TURN_CHECK_SECONDS = 2;
    private static readonly object _lock = new object();
    private static Instant? _nextTurnExpiryCheck;

    private readonly BackgroundQueue _backgroundQueue;
    private readonly IClock _clock;
    private readonly GameRepository _gameRepository;
    private readonly ILogger<HeartBeatMiddleware> _log;
    private readonly RequestDelegate _next;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<WebsocketHub> _websocketHub;

    public HeartBeatMiddleware(BackgroundQueue backgroundQueue, IClock clock, GameRepository gameRepository,
            ILogger<HeartBeatMiddleware> log, RequestDelegate next, IUnitOfWork unitOfWork,
            IHubContext<WebsocketHub> websocketHub) {
        _backgroundQueue = backgroundQueue;
        _clock = clock;
        _gameRepository = gameRepository;
        _log = log;
        _next = next;
        _unitOfWork = unitOfWork;
        _websocketHub = websocketHub;
    }

    public async Task Invoke(HttpContext context) {
        try {
            var now = _clock.GetCurrentInstant();
            bool handleMissingTurns = false;
            lock(_lock) {
                if (_nextTurnExpiryCheck is null || _nextTurnExpiryCheck.Value <= now) {
                    _nextTurnExpiryCheck = now.Plus(Duration.FromSeconds(TURN_CHECK_SECONDS));
                    handleMissingTurns = true;
                }
            }

            if (handleMissingTurns) {
                var gameIds = _unitOfWork.WithGameTransaction(tran => {
                    return _gameRepository.ListGameIdsForExpiredTurns(now.InUtc());
                });
                _backgroundQueue.Enqueue(async cancellationToken => await HandleMissingTurns(gameIds, now.InUtc()));
            }
        }
        catch (Exception e) {
            _log.LogError("Uncaught exception in the HeartBeat middleware", e);
        }

        await _next(context);
    }

    private async Task HandleMissingTurns(IReadOnlyList<GameId> gameIds, ZonedDateTime now) {
        foreach (var gameId in gameIds) {
            try {
                _unitOfWork.WithGameTransaction(tran => {
                    var game = _gameRepository.FindById(gameId);
                    game.MissTurn(now);
                    _gameRepository.Save(game);
                });

                await _websocketHub.PushGameUpdateReady(gameId);
            }
            catch (Exception e) {
                _log.LogError($"HeartBeat HandleMissingTurns exception for game {gameId}", e);
            }
        }
    }
}
