using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.GameContext.Models.Voting;
using static Draughts.Domain.UserContext.Models.Rank;

namespace Draughts.Repositories.Misc;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
public sealed class DbUser : IDbObject<DbUser, User> {
    public long Id { get; set; }
    public string Username { get; set; }
    public int Rating { get; set; }
    public string Rank { get; set; }
    public ZonedDateTime CreatedAt { get; set; }
    public int TotalPlayed { get; set; }
    public int TotalWon { get; set; }
    public int TotalTied { get; set; }
    public int TotalLost { get; set; }
    public int InternationalPlayed { get; set; }
    public int InternationalWon { get; set; }
    public int InternationalTied { get; set; }
    public int InternationalLost { get; set; }
    public int EnglishAmericanPlayed { get; set; }
    public int EnglishAmericanWon { get; set; }
    public int EnglishAmericanTied { get; set; }
    public int EnglishAmericanLost { get; set; }
    public int OtherPlayed { get; set; }
    public int OtherWon { get; set; }
    public int OtherTied { get; set; }
    public int OtherLost { get; set; }

    public bool Equals(DbUser? other) => Id.Equals(other?.Id);

    public User ToDomainModel() {
        var userId = new UserId(Id);
        return new User(
            userId,
            new Username(Username),
            new Rating(Rating),
            Ranks.All.Single(r => r.Name == Rank),
            new UserStatistics(
                userId,
                new GamesTally(TotalPlayed, TotalWon, TotalTied, TotalLost),
                new GamesTally(InternationalPlayed, InternationalWon, InternationalTied, InternationalLost),
                new GamesTally(EnglishAmericanPlayed, EnglishAmericanWon, EnglishAmericanTied, EnglishAmericanLost),
                new GamesTally(OtherPlayed, OtherWon, OtherTied, OtherLost)
            ),
            CreatedAt
        );
    }

    public static DbUser FromDomainModel(User entity) {
        return new DbUser {
            Id = entity.Id.Value,
            Username = entity.Username.Value,
            Rating = entity.Rating.Value,
            Rank = entity.Rank.Name,
            TotalPlayed = entity.Statistics.TotalTally.Played,
            TotalWon = entity.Statistics.TotalTally.Won,
            TotalTied = entity.Statistics.TotalTally.Tied,
            TotalLost = entity.Statistics.TotalTally.Lost,
            InternationalPlayed = entity.Statistics.InternationalTally.Played,
            InternationalWon = entity.Statistics.InternationalTally.Won,
            InternationalTied = entity.Statistics.InternationalTally.Tied,
            InternationalLost = entity.Statistics.InternationalTally.Lost,
            EnglishAmericanPlayed = entity.Statistics.EnglishAmericanTally.Played,
            EnglishAmericanWon = entity.Statistics.EnglishAmericanTally.Won,
            EnglishAmericanTied = entity.Statistics.EnglishAmericanTally.Tied,
            EnglishAmericanLost = entity.Statistics.EnglishAmericanTally.Lost,
            OtherPlayed = entity.Statistics.OtherTally.Played,
            OtherWon = entity.Statistics.OtherTally.Won,
            OtherTied = entity.Statistics.OtherTally.Tied,
            OtherLost = entity.Statistics.OtherTally.Lost,
            CreatedAt = entity.CreatedAt
        };
    }
}

public sealed class DbAuthUser : IDbObject<DbAuthUser, AuthUser> {
    public long Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public ZonedDateTime CreatedAt { get; set; }

    public bool Equals(DbAuthUser? other) => Id.Equals(other?.Id);

    public AuthUser ToDomainModel(IEnumerable<RoleId> roles) {
        return new AuthUser(
            new UserId(Id),
            new Username(Username),
            Domain.AuthContext.Models.PasswordHash.FromStorage(PasswordHash),
            new Email(Email),
            CreatedAt,
            roles
        );
    }

    public static DbAuthUser FromDomainModel(AuthUser entity) {
        return new DbAuthUser {
            Id = entity.Id.Value,
            Username = entity.Username.Value,
            PasswordHash = entity.PasswordHash.ToStorage(),
            Email = entity.Email.Value,
            CreatedAt = entity.CreatedAt
        };
    }
}

public sealed class DbAuthUserRole : IEquatable<DbAuthUserRole> {
    public long UserId { get; set; }
    public long RoleId { get; set; }

    public bool Equals(DbAuthUserRole? other) {
        return other is not null && other.UserId == UserId && other.RoleId == RoleId;
    }
}

public sealed class DbRole : IDbObject<DbRole, Role> {
    public long Id { get; set; }
    public string Rolename { get; set; }
    public ZonedDateTime CreatedAt { get; set; }

    public bool Equals(DbRole? other) => Id.Equals(other?.Id);

    public Role ToDomainModel(Permission[] permissions) {
        return new Role(new RoleId(Id), Rolename, CreatedAt, permissions);
    }

    public static DbRole FromDomainModel(Role entity) {
        return new DbRole {
            Id = entity.Id.Value,
            Rolename = entity.Rolename,
            CreatedAt = entity.CreatedAt
        };
    }
}

public sealed class DbPermissionRole : IEquatable<DbPermissionRole> {
    public long RoleId { get; set; }
    public string Permission { get; set; }

    public bool Equals(DbPermissionRole? other) {
        return other is not null && other.RoleId == RoleId && other.Permission == Permission;
    }
}

public sealed class DbAdminLog : IDbObject<DbAdminLog, AdminLog> {
    public long Id { get; set; }
    public string Type { get; set; }
    public string Parameters { get; set; }
    public long UserId { get; set; }
    public string Username { get; set; }
    public string Permission { get; set; }
    public ZonedDateTime CreatedAt { get; set; }

    public bool Equals(DbAdminLog? other) => Id.Equals(other?.Id);

    public AdminLog ToDomainModel() {
        return new AdminLog(
            new AdminLogId(Id),
            Type,
            Parameters.Split(','),
            new UserId(UserId),
            new Username(Username),
            new Permission(Permission),
            CreatedAt
        );
    }

    public static DbAdminLog FromDomainModel(AdminLog entity) {
        return new DbAdminLog {
            Id = entity.Id.Value,
            Type = entity.Type,
            Parameters = string.Join(',', entity.Parameters),
            UserId = entity.UserId.Value,
            Username = entity.Username.Value,
            Permission = entity.Permission.Value,
            CreatedAt = entity.CreatedAt
        };
    }
}

public sealed class DbGame : IDbObject<DbGame, Game> {
    public long Id { get; set; }
    public int BoardSize { get; set; }
    public bool FirstMoveColorIsWhite { get; set; }
    public bool FlyingKings { get; set; }
    public bool MenCaptureBackwards { get; set; }
    public string CaptureConstraints { get; set; }
    public int TurnTime { get; set; }
    public long? Victor { get; set; }
    public ZonedDateTime CreatedAt { get; set; }
    public ZonedDateTime? StartedAt { get; set; }
    public ZonedDateTime? FinishedAt { get; set; }
    public long? TurnPlayerId { get; set; }
    public ZonedDateTime? TurnCreatedAt { get; set; }
    public ZonedDateTime? TurnExpiresAt { get; set; }

    public bool Equals(DbGame? other) => Id.Equals(other?.Id);

    public Game ToDomainModel(List<Player> players) {
        return new Game(
            new GameId(Id),
            players,
            GetTurn(players),
            GetGameSettings(),
            players.SingleOrDefault(p => p.UserId == Victor),
            CreatedAt,
            StartedAt,
            FinishedAt
        );
    }

    private Turn? GetTurn(List<Player> players) {
        return TurnPlayerId is null || TurnCreatedAt is null || TurnExpiresAt is null ? null : new Turn(
            players.Single(p => p.Id == TurnPlayerId.Value),
            TurnCreatedAt.Value,
            TurnExpiresAt.Value - TurnCreatedAt.Value
        );
    }

    public GameSettings GetGameSettings() {
        Color firstMoveColor = FirstMoveColorIsWhite ? Color.White : Color.Black;
        var capConstraints = CaptureConstraints switch {
            "max" => GameSettings.DraughtsCaptureConstraints.MaximumPieces,
            "seq" => GameSettings.DraughtsCaptureConstraints.AnyFinishedSequence,
            _ => throw new InvalidOperationException("Unknown capture constraint.")
        };
        return new GameSettings(BoardSize, firstMoveColor, FlyingKings, MenCaptureBackwards,
            capConstraints, Duration.FromSeconds(TurnTime));
    }

    public static DbGame FromDomainModel(Game entity) {
        string captureConstraints = entity.Settings.CaptureConstraints switch {
            GameSettings.DraughtsCaptureConstraints.AnyFinishedSequence => "seq",
            GameSettings.DraughtsCaptureConstraints.MaximumPieces => "max",
            _ => throw new InvalidOperationException("Unknown capture constraint")
        };
        return new DbGame {
            Id = entity.Id.Value,
            BoardSize = entity.Settings.BoardSize,
            FirstMoveColorIsWhite = entity.Settings.FirstMove == Color.White,
            FlyingKings = entity.Settings.FlyingKings,
            MenCaptureBackwards = entity.Settings.MenCaptureBackwards,
            CaptureConstraints = captureConstraints,
            TurnTime = (int)entity.Settings.MaxTurnLength.TotalSeconds,
            Victor = entity.Victor?.UserId.Value,
            CreatedAt = entity.CreatedAt,
            StartedAt = entity.StartedAt,
            FinishedAt = entity.FinishedAt,
            TurnPlayerId = entity.Turn?.Player.Id.Value,
            TurnCreatedAt = entity.Turn?.CreatedAt,
            TurnExpiresAt = entity.Turn?.ExpiresAt
        };
    }
}

public sealed class DbPlayer : IDbObject<DbPlayer, Player> {
    public long Id { get; set; }
    public long UserId { get; set; }
    public long GameId { get; set; }
    public string Username { get; set; }
    public string Rank { get; set; }
    public bool Color { get; set; }
    public ZonedDateTime CreatedAt { get; set; }

    public bool Equals(DbPlayer? other) => Id.Equals(other?.Id);

    public Player ToDomainModel() {
        return new Player(
            new PlayerId(Id),
            new UserId(UserId),
            new Username(Username),
            Ranks.All.Single(r => r.Name == Rank),
            Color ? Domain.GameContext.Models.Color.White : Domain.GameContext.Models.Color.Black,
            CreatedAt
        );
    }

    public static DbPlayer FromDomainModel(Player entity, GameId gameId) {
        return new DbPlayer {
            Id = entity.Id.Value,
            UserId = entity.UserId.Value,
            GameId = gameId.Value,
            Username = entity.Username.Value,
            Rank = entity.Rank.Name,
            Color = entity.Color == Domain.GameContext.Models.Color.White,
            CreatedAt = entity.CreatedAt
        };
    }
}

public sealed class DbGameState : IDbObject<DbGameState, GameState> {
    public long Id { get; set; }
    public string? InitialGameState { get; set; }

    public bool Equals(DbGameState? other) => Id.Equals(other?.Id);

    public GameState ToDomainModel(GameSettings settings, IEnumerable<DbMove> dbMoves) {
        var moves = dbMoves.Select(m => m.ToDomainModel());
        return GameState.FromStorage(new GameId(Id), settings, InitialGameState, moves);
    }

    public static DbGameState FromDomainModel(GameState entity) {
        return new DbGameState {
            Id = entity.Id.Value,
            InitialGameState = entity.InitialStateStorageString()
        };
    }
}

public sealed class DbMove : IDbObject<DbMove, Move> {
    public long GameId { get; set; }
    public short Index { get; set; }
    public byte From { get; set; }
    public byte To { get; set; }
    public bool IsCapture { get; set; }

    public bool Equals(DbMove? other) => GameId.Equals(other?.GameId) && Index.Equals(other?.Index);

    public Move ToDomainModel() => new Move(new SquareId(From), new SquareId(To), IsCapture);

    public static DbMove FromDomainModel(GameState gameState, Move move, int moveIndex) {
        return new DbMove {
            GameId = gameState.Id.Value,
            Index = Convert.ToInt16(moveIndex),
            From = Convert.ToByte(move.From.Value),
            To = Convert.ToByte(move.To.Value),
            IsCapture = move.IsCapture
        };
    }

    public static DbMove[] ArrayFromDomainModels(GameState gameState, int currentMaxIndex) {
        var moves = new DbMove[gameState.Moves.Count - currentMaxIndex - 1];
        for (int i = 0; i < moves.Length; i++) {
            int moveIndex = i + currentMaxIndex + 1;
            moves[i] = FromDomainModel(gameState, gameState.Moves[moveIndex], moveIndex);
        }
        return moves;
    }
}

public sealed class DbVote : IEquatable<DbVote> {
    public long GameId { get; set; }
    public long UserId { get; set; }
    public string Subject { get; set; }
    public bool InFavor { get; set; }
    public ZonedDateTime CreatedAt { get; set; }

    public static IReadOnlyList<Voting> ToDomainModels(IEnumerable<DbVote> dbVotes) {
        var votesPerGame = dbVotes.GroupBy(v => v.GameId);
        var results = new List<Voting>();
        foreach (var gameVotes in votesPerGame) {
            var votes = gameVotes
                .Select(v => new Vote(new UserId(v.UserId), VotingSubjectFromString(v.Subject), v.InFavor, v.CreatedAt))
                .ToList();
            results.Add(new Voting(new GameId(gameVotes.Key), votes));
        }
        return results;
    }

    private static VotingSubject VotingSubjectFromString(string votingSubject) => votingSubject switch {
        "draw" => VotingSubject.Draw,
        _ => throw new InvalidOperationException("Unknown voting subject")
    };

    public static List<DbVote> FromDomainModel(Voting voting) {
        return voting.Votes.Select(v => new DbVote {
            GameId = voting.Id.Value,
            UserId = v.UserId.Value,
            Subject = VotingSubjectToString(v.Subject),
            InFavor = v.InFavor,
            CreatedAt = v.CreatedAt
        }).ToList();
    }

    private static string VotingSubjectToString(VotingSubject votingSubject) => votingSubject switch {
        VotingSubject.Draw => "draw",
        _ => throw new InvalidOperationException("Unknown voting subject")
    };

    public override bool Equals(object? obj) => Equals(obj as DbVote);
    public bool Equals(DbVote? other) {
        return other?.GameId == GameId
            && other?.UserId == UserId
            && other?.Subject == Subject
            && other?.InFavor == InFavor
            && other?.CreatedAt == CreatedAt;
    }

    public override int GetHashCode() {
        return ComparisonUtils.GetHashCode(new object[] { GameId, UserId, Subject, InFavor, CreatedAt });
    }
}

public sealed class DbEvent : IDbObject<DbEvent, DomainEvent> {
    public long Id { get; set; }
    public string Type { get; set; }
    public ZonedDateTime CreatedAt { get; set; }
    public ZonedDateTime? HandledAt { get; set; }
    public string Data { get; set; }

    public bool Equals(DbEvent? other) => other?.Id == Id;

    public static DomainEvent ToDomainModel(DbEvent e) {
        return e.Type switch
        {
            AuthUserCreated.TYPE => AuthUserCreated.FromStorage(GetId(e), e.CreatedAt, e.HandledAt, e.Data),
            RoleCreated.TYPE => RoleCreated.FromStorage(GetId(e), e.CreatedAt, e.HandledAt, e.Data),
            RoleEdited.TYPE => RoleEdited.FromStorage(GetId(e), e.CreatedAt, e.HandledAt, e.Data),
            RoleDeleted.TYPE => RoleDeleted.FromStorage(GetId(e), e.CreatedAt, e.HandledAt, e.Data),
            UserLostRole.TYPE => UserLostRole.FromStorage(GetId(e), e.CreatedAt, e.HandledAt, e.Data),
            UserGainedRole.TYPE => UserGainedRole.FromStorage(GetId(e), e.CreatedAt, e.HandledAt, e.Data),
            GameFinished.TYPE => GameFinished.FromStorage(GetId(e), e.CreatedAt, e.HandledAt, e.Data),
            UserCreated.TYPE => UserCreated.FromStorage(GetId(e), e.CreatedAt, e.HandledAt, e.Data),
            _ => throw new InvalidOperationException($"Unknown event type '{e.Type}' for event '{e.Id}'")
        };
    }

    private static DomainEventId GetId(DbEvent dbEvent) => new DomainEventId(dbEvent.Id);

    public static DbEvent FromDomainModel(DomainEvent entity) {
        return new DbEvent {
            Id = entity.Id.Value,
            Type = entity.Type,
            CreatedAt = entity.CreatedAt,
            HandledAt = entity.HandledAt,
            Data = entity.BuildDataString()
        };
    }
}

public sealed class DbReceivedEvent : IEquatable<DbReceivedEvent> {
    public long Id { get; set; }
    public ZonedDateTime HandledAt { get; set; }

    public override bool Equals(object? obj) => obj is DbReceivedEvent evt && Equals(evt);
    public bool Equals(DbReceivedEvent? other) => other?.Id == Id;
    public override int GetHashCode() => Id.GetHashCode();
}

public sealed class DbIdGeneration {
    public const string SUBJECT_MISC = "";
    public const string SUBJECT_GAME = "game";
    public const string SUBJECT_USER = "user";

    public string Subject { get; set; }
    public long AvailableId { get; set; }

    public DbIdGeneration() { }
    public DbIdGeneration(string subject, long id) {
        Subject = subject;
        AvailableId = id;
    }

    public bool Equals(DbIdGeneration? other) => AvailableId.Equals(other?.AvailableId) && Subject.Equals(other?.Subject);
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
