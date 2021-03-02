using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Application.Auth.Services {
    public class AuthUserFactory : IAuthUserFactory {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IClock _clock;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthUserFactory(IAuthUserRepository authUserRepository, IClock clock, IRoleRepository roleRepository,
                IUnitOfWork unitOfWork) {
            _authUserRepository = authUserRepository;
            _clock = clock;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public AuthUser CreateAuthUser(IIdPool idPool, string? name, string? email, string? plaintextPassword) {
            var nextUserId = new UserId(idPool.NextForUser());
            var username = new Username(name);
            var passwordHash = PasswordHash.Generate(plaintextPassword, nextUserId, username);

            var pendingRestrationRole = _roleRepository.Find(new RolenameSpecification(Role.PENDING_REGISTRATION_ROLENAME));

            var authUser = new AuthUser(nextUserId, username, passwordHash, new Email(email), _clock.UtcNow(),
                new[] { pendingRestrationRole });

            _authUserRepository.Save(authUser);

            _unitOfWork.Raise(AuthUserCreated.Factory(authUser));

            return authUser;
        }

        public AuthUser FinishRegistration(UserId id) {
            var registeredUserRole = _roleRepository.Find(new RolenameSpecification(Role.REGISTERED_USER_ROLENAME));
            var authUser = _authUserRepository.FindById(id);

            authUser.Register(registeredUserRole);

            _authUserRepository.Save(authUser);

            return authUser;
        }
    }
}
