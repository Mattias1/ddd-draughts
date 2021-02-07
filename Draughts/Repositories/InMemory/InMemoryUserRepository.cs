using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Domain.UserAggregate.Models.Rank;

namespace Draughts.Repositories.InMemory {
    public class InMemoryUserRepository : InMemoryRepository<User>, IUserRepository {
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryUserRepository(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        protected override IList<User> GetBaseQuery() {
            return UserDatabase.UsersTable.Select(u => new User(
                new UserId(u.Id),
                new AuthUserId(u.AuthUserId),
                new Username(u.Username),
                new Rating(u.Rating),
                Ranks.All.Single(r => r.Name == u.Rank),
                u.GamesPlayed,
                u.CreatedAt)
            ).ToList();
        }

        public User FindById(UserId id) => Find(new UserIdSpecification(id));
        public User? FindByIdOrNull(UserId id) => FindOrNull(new UserIdSpecification(id));

        public override void Save(User entity) {
            var user = new InMemoryUser {
                Id = entity.Id,
                AuthUserId = entity.AuthUserId,
                Username = entity.Username,
                Rating = entity.Rating,
                Rank = entity.Rank.Name,
                GamesPlayed = entity.GamesPlayed,
                CreatedAt = entity.CreatedAt
            };

            _unitOfWork.Store(user, UserDatabase.TempUsersTable);
        }
    }
}
