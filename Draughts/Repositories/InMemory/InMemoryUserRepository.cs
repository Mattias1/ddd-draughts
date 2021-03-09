using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public class InMemoryUserRepository : InMemoryRepository<User, UserId>, IUserRepository {
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryUserRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        protected override IList<User> GetBaseQuery() {
            return UserDatabase.Get.UsersTable.Select(u => u.ToDomainModel()).ToList();
        }

        public override void Save(User entity) {
            var user = DbUser.FromDomainModel(entity);
            _unitOfWork.Store(user, tran => UserDatabase.Temp(tran).UsersTable);
        }
    }
}
