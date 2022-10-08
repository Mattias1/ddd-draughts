using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.AuthContext.Events;

public sealed class GameFinished : DomainEvent {
    public const string TYPE = "game.finished";

    public GameId GameId { get; }
    public UserId? Victor { get; }
    private UserId[] _players;
    public GameSettings.GameSettingsPreset SettingsPreset { get; }

    public IReadOnlyList<UserId> Players => _players.ToList().AsReadOnly();

    public GameFinished(GameId gameId, UserId[] players, UserId? victor, GameSettings.GameSettingsPreset settingsPreset,
            DomainEventId id, ZonedDateTime createdAt, ZonedDateTime? handledAt)
            : base(id, TYPE, createdAt, handledAt) {
        GameId = gameId;
        _players = players;
        Victor = victor;
        SettingsPreset = settingsPreset;
    }

    public override string BuildDataString() {
        var players = _players.Select(id => id.Value).ToArray();
        return JsonUtils.SerializeEvent(Id, new EventData(GameId.Value, players, Victor?.Value, SettingsPreset));
    }

    public static DomainEventFactory Factory(Game game) {
        return (id, now) => {
            var players = game.Players.Select(p => p.UserId).ToArray();
            var victor = game.Victor?.UserId;
            var settingsPreset = game.Settings.PresetEnum;
            return new GameFinished(game.Id, players, victor, settingsPreset, id, now, null);
        };
    }

    public static GameFinished FromStorage(DomainEventId id,
            ZonedDateTime createdAt, ZonedDateTime? handledAt, string dataString) {
        var data = JsonUtils.DeserializeEvent<EventData>(id, dataString);
        var players = data.Players.Select(id => new UserId(id)).ToArray();
        return new GameFinished(new GameId(data.GameId), players, UserId.FromNullable(data.Victor),
            data.SettingsPreset, id, createdAt, handledAt);
    }

    private readonly record struct EventData(long GameId, long[] Players, long? Victor,
            GameSettings.GameSettingsPreset SettingsPreset);
}
