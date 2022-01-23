using Draughts.Application.Shared.ViewModels;
using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System;
using System.Threading.Tasks;

namespace SignalRWebPack.Hubs;

public class WebsocketHub : Hub {
    public async Task AssociateGame(object? rawGameId) {
        try {
            var gameId = rawGameId?.ToString();
            if (!int.TryParse(gameId, out _)) {
                throw new ManualValidationException("Invalid connection attempt while associating for game " + rawGameId);
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, GameGroup(gameId?.ToString()));
            Log.Debug("Websocket connection associated with game id " + gameId);
        }
        catch (Exception e) {
            Log.Error(e.Message);
            throw;
        }
    }

    public static string GameGroup(GameId gameId) => GameGroup(gameId.ToString());
    public static string GameGroup(string? gameId) => $"game-{gameId}";
}

public static class WebsocketHubExtensions {
    public static async Task PushGameUpdated(this IHubContext<WebsocketHub> websocketHubContext, GameId gameId, GameDto data) {
        try {
            await websocketHubContext.Clients.Group(WebsocketHub.GameGroup(gameId)).SendAsync("gameUpdated", data);
            Log.Debug("Pushed websocket game update for game id " + gameId.ToString());
        }
        catch (Exception e) {
            Log.Error(e.Message);
            throw;
        }
    }
}
