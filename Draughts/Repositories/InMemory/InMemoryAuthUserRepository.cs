using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public class InMemoryAuthUserRepository : InMemoryRepository<AuthUser>, IAuthUserRepository {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryAuthUserRepository(IRoleRepository roleRepository, IUnitOfWork unitOfWork) {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        protected override IList<AuthUser> GetBaseQuery() {
            var roles = _roleRepository.List().ToDictionary(r => r.Id.Id);
            return AuthUserDatabase.AuthUsersTable.Select(u => new AuthUser(
                new AuthUserId(u.Id),
                new UserId(u.UserId),
                new Username(u.Username),
                PasswordHash.FromStorage(u.PasswordHash),
                new Email(u.Email),
                u.CreatedAt,
                u.RoleIds.Select(r => roles[r]).ToList()
            )).ToList();
        }

        public AuthUser FindById(AuthUserId id) => Find(new AuthUserIdSpecification(id));
        public AuthUser? FindByIdOrNull(AuthUserId id) => FindOrNull(new AuthUserIdSpecification(id));

        public override void Save(AuthUser entity) {
            var authUser = new InMemoryAuthUser {
                Id = entity.Id,
                UserId = entity.UserId,
                Username = entity.Username,
                PasswordHash = entity.PasswordHash.ToStorage(),
                Email = entity.Email,
                CreatedAt = entity.CreatedAt,
                RoleIds = entity.Roles.Select(r => r.Id.Id).ToArray()
            };

            _unitOfWork.Store(authUser, AuthUserDatabase.TempAuthUsersTable);
        }
    }
}
