using Draughts.Application.Shared.ViewModels;
using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Draughts.Application.Hubs;

public sealed class WebsocketHub : Hub {
    public async Task AssociateGame(object? rawGameId) {
        try {
            var gameId = rawGameId?.ToString();
            if (!int.TryParse(gameId, out _)) {
                throw new ManualValidationException("Invalid connection attempt while associating for game " + rawGameId);
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, GameGroup(gameId));
            Log.Debug("Websocket connection associated with game " + gameId);
        } catch (Exception e) {
            Log.Error(e, e.Message);
            throw;
        }
    }

    public static string GameGroup(GameId gameId) => GameGroup(gameId.ToString());
    public static string GameGroup(string? gameId) => $"game-{gameId}";
}

public static class WebsocketHubExtensions {
    public static async Task PushGameUpdateReady(this IHubContext<WebsocketHub> websocketHubContext, GameId gameId) {
        try {
            await websocketHubContext.Clients.Group(WebsocketHub.GameGroup(gameId)).SendAsync("gameUpdateReady", gameId.ToString());
            Log.Debug($"Pushed websocket game update ready notification for game {gameId}");
        } catch (Exception e) {
            Log.Error(e, e.Message);
            throw;
        }
    }

    public static async Task PushGameUpdated(this IHubContext<WebsocketHub> websocketHubContext, GameId gameId, GameDto data) {
        try {
            await websocketHubContext.Clients.Group(WebsocketHub.GameGroup(gameId)).SendAsync("gameUpdated", data);
            Log.Debug($"Pushed websocket game update with data for game {gameId}");
        } catch (Exception e) {
            Log.Error(e, e.Message);
            throw;
        }
    }
}
