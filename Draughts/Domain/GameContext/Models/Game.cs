using Draughts.Common;
using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.GameContext.Models;

public sealed class Game : AggregateRoot<Game, GameId> {
    public const string ERROR_GAME_NOT_ACTIVE = "This game is not active.";
    public const string ERROR_NOT_YOUR_TURN = "It's not your turn.";
    public const string ERROR_NOT_IN_GAME = "You're not in the game.";

    private readonly List<Player> _players;

    public override GameId Id { get; }
    public IReadOnlyList<Player> Players => _players.AsReadOnly();
    public Turn? Turn { get; private set; }
    public GameSettings Settings { get; private set; }
    public Player? Victor { get; private set; }
    public ZonedDateTime CreatedAt { get; }
    public ZonedDateTime? StartedAt { get; private set; }
    public ZonedDateTime? FinishedAt { get; private set; }

    public bool HasStarted => StartedAt is not null;
    public bool IsFinished => FinishedAt is not null;

    public Game(GameId id, GameSettings settings, ZonedDateTime createdAt)
        : this(id, new List<Player>(2), null, settings, null,
            createdAt, null, null) { }
    public Game(GameId id, List<Player> players, Turn? turn, GameSettings settings, Player? victor,
            ZonedDateTime createdAt, ZonedDateTime? startedAt, ZonedDateTime? finishedAt) {
        Id = id;
        _players = players;
        Turn = turn;
        Settings = settings;
        Victor = victor;
        CreatedAt = createdAt;
        StartedAt = startedAt;
        FinishedAt = finishedAt;
    }

    public void JoinGame(Player player, ZonedDateTime joinedAt) {
        if (HasStarted) {
            throw new ManualValidationException("Cannot join a game that is already started.");
        }
        if (_players.Contains(player)) {
            throw new ManualValidationException("Cannot join a game you have already joined.");
        }
        if (_players.Any(p => p.UserId == player.UserId)) {
            throw new ManualValidationException($"Cannot join a game you have already joined.");
        }
        if (_players.Any(p => p.Color == player.Color)) {
            throw new ManualValidationException($"The color {player.Color} is already taken.");
        }

        _players.Add(player);

        if (_players.Count == 2) {
            StartGame(joinedAt);
        }
    }

    private void StartGame(ZonedDateTime startedAt) {
        StartedAt = startedAt;
        var startingPlayer = GetPlayerForColor(Settings.FirstMove);
        if (_players[0] != startingPlayer) {
            _players[1] = _players[0];
            _players[0] = startingPlayer;
        }
        SwitchTurn(startedAt);
    }

    public void NextTurn(UserId currentUser, ZonedDateTime switchedAt) {
        ValidateCanDoMove(currentUser);
        SwitchTurn(switchedAt);
    }

    public void MissTurn(ZonedDateTime expiredAt) {
        ValidateGameIsActive();
        if (Turn is null || Turn.ExpiresAt.ToInstant() > expiredAt.ToInstant()) {
            throw new ManualValidationException("You cannot miss a turn unless the turn exists and is expired.");
        }
        SwitchTurn(expiredAt);
    }

    private void SwitchTurn(ZonedDateTime switchedAt) {
        var player = Turn is null ? GetPlayerForColor(Settings.FirstMove) : Players.Single(p => p != Turn.Player);
        Turn = new Turn(player, switchedAt, Settings.MaxTurnLength);
    }

    public void ChangeTurnTime(ZonedDateTime now, int turnTimeInSeconds, bool forAllTurns) {
        ValidateGameIsActive();
        var test = now.PlusSeconds(turnTimeInSeconds);
        var test2 = now.PlusHours(48);
        Turn = Turn!.WithExpiry(now.PlusSeconds(turnTimeInSeconds));
        if (forAllTurns) {
            Settings = Settings.WithTurnTime(Duration.FromSeconds(turnTimeInSeconds));
        }
    }

    private Player GetPlayerForColor(Color color) => _players.Single(p => p.Color == color);

    public void WinGame(UserId currentUser, ZonedDateTime finishedAt) {
        ValidateCanDoMove(currentUser);

        FinishedAt = finishedAt;
        Victor = Turn!.Player;
        Turn = null;

        AttachEvent(GameFinished.Factory(this));
    }

    public void Draw(ZonedDateTime finishedAt) {
        ValidateGameIsActive();

        FinishedAt = finishedAt;
        Victor = null;
        Turn = null;

        AttachEvent(GameFinished.Factory(this));
    }

    public void ResignGame(UserId currentUser, ZonedDateTime finishedAt) {
        ValidateGameIsActive();

        FinishedAt = finishedAt;
        Victor = Players.Single(p => p.UserId != currentUser);
        Turn = null;

        AttachEvent(GameFinished.Factory(this));
    }

    public void ValidateCanDoMove(UserId currentUser) {
        ValidateGameIsActive();
        if (Turn!.Player.UserId != currentUser) {
            throw new ManualValidationException(ERROR_NOT_YOUR_TURN);
        }
    }

    public void ValidateCanVote(UserId currentUser) {
        ValidateGameIsActive();
        if (!Players.Any(p => p.UserId == currentUser)) {
            throw new ManualValidationException(ERROR_NOT_IN_GAME);
        }
    }

    private void ValidateGameIsActive() {
        if (!HasStarted || IsFinished || Turn is null) {
            throw new ManualValidationException(ERROR_GAME_NOT_ACTIVE);
        }
    }
}
