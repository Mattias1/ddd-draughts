using Draughts.Common;
using Draughts.Common.OoConcepts;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.GameContext.Models.Voting;

namespace Draughts.Domain.GameContext.Models {
    public class Voting : Entity<Voting, GameId> {
        public enum VotingSubject { Draw }

        private readonly List<Vote> _votes;

        public override GameId Id { get; }
        public IReadOnlyList<Vote> Votes => _votes.AsReadOnly();

        public Voting(GameId id, List<Vote> votes) {
            Id = id;
            _votes = votes;
        }

        public bool AreAllInFavor(VotingSubject subject) {
            return _votes.Count(v => v.Subject == subject && v.InFavor) == 2;
        }

        public void VoteFor(UserId currentUser, VotingSubject subject, ZonedDateTime votedAt) {
            if (_votes.Any(v => v.UserId == currentUser && v.Subject == subject)) {
                throw new ManualValidationException("You've already voted on this subject.");
            }
            _votes.Add(new Vote(currentUser, subject, true, votedAt));
        }

        public static Voting StartNew(GameId gameId) => new Voting(gameId, new List<Vote>());
    }

    public class Vote : ValueObject<Vote> {
        public UserId UserId { get; }
        public VotingSubject Subject { get; }
        public bool InFavor { get; }
        public ZonedDateTime CreatedAt { get; }

        public Vote(UserId userId, VotingSubject subject, bool inFavor, ZonedDateTime createdAt) {
            UserId = userId;
            Subject = subject;
            InFavor = inFavor;
            CreatedAt = createdAt;
        }

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return UserId;
            yield return Subject;
            yield return InFavor;
            yield return CreatedAt;
        }
    }
}
