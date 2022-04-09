using Draughts.Common.Events;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;
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

    public GameFinished(Game game, DomainEventId id, ZonedDateTime createdAt) : base(id, TYPE, createdAt) {
        GameId = game.Id;
        _players = game.Players.Select(p => p.UserId).ToArray();
        Victor = game.Victor?.UserId;
        SettingsPreset = game.Settings.PresetEnum;
    }

    public static Func<DomainEventId, ZonedDateTime, GameFinished> Factory(Game game) {
        return (id, now) => new GameFinished(game, id, now);
    }
}
