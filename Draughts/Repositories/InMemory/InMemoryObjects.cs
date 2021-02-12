using Draughts.Domain.GameAggregate.Models;
using NodaTime;
using System;

namespace Draughts.Repositories.InMemory {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    public class InMemoryUser : IEquatable<InMemoryUser> {
        public long Id;
        public long AuthUserId;
        public string Username;
        public int Rating;
        public string Rank;
        public int GamesPlayed;
        public ZonedDateTime CreatedAt;

        public bool Equals(InMemoryUser? other) => Id.Equals(other?.Id);
    }

    public class InMemoryAuthUser : IEquatable<InMemoryAuthUser> {
        public long Id;
        public long UserId;
        public string Username;
        public string PasswordHash;
        public string Email;
        public long[] RoleIds;
        public ZonedDateTime CreatedAt;

        public bool Equals(InMemoryAuthUser? other) => Id.Equals(other?.Id);
    }

    public class InMemoryRole : IEquatable<InMemoryRole> {
        public long Id;
        public string Rolename;
        public string[] Permissions;
        public ZonedDateTime CreatedAt;

        public bool Equals(InMemoryRole? other) => Id.Equals(other?.Id);
    }

    public class InMemoryGame : IEquatable<InMemoryGame> {
        public long Id;
        public int BoardSize;
        public bool FirstMoveColorIsWhite;
        public bool FlyingKings;
        public bool MenCaptureBackwards;
        public GameSettings.DraughtsCaptureConstraints CaptureConstraints;
        public long? Victor;
        public string CurrentGameState;
        public int? CaptureSequenceFrom;
        public ZonedDateTime CreatedAt;
        public ZonedDateTime? StartedAt;
        public ZonedDateTime? FinishedAt;
        public long? TurnPlayerId;
        public ZonedDateTime? TurnCreatedAt;
        public ZonedDateTime? TurnExpiresAt;
        public long[] PlayerIds;

        public bool Equals(InMemoryGame? other) => Id.Equals(other?.Id);
    }

    public class InMemoryPlayer : IEquatable<InMemoryPlayer> {
        public long Id;
        public long UserId;
        public string Username;
        public string Rank;
        public bool ColorIsWhite;
        public ZonedDateTime CreatedAt;

        public bool Equals(InMemoryPlayer? other) => Id.Equals(other?.Id);
    }

    public class InMemoryAvailableId : IEquatable<InMemoryAvailableId> {
        public long Id { get; set; }

        public bool Equals(InMemoryAvailableId? other) => Id.Equals(other?.Id);
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
}
