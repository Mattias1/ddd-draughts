using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;

namespace Draughts.Services {
    public class AuthUserFactory : IAuthUserFactory {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IEventQueue _eventQueue;
        private readonly IIdGenerator _idGenerator;
        private readonly IRoleRepository _roleRepository;

        public AuthUserFactory(
            IAuthUserRepository authUserRepository, IEventQueue eventQueue,
            IIdGenerator idGenerator, IRoleRepository roleRepository
        ) {
            _authUserRepository = authUserRepository;
            _eventQueue = eventQueue;
            _idGenerator = idGenerator;
            _roleRepository = roleRepository;
        }

        public AuthUser CreateAuthUser(string? name, string? email, string? plaintextPassword) {
            var nextId = new AuthUserId(_idGenerator.Next());
            var nextUserId = new UserId(_idGenerator.Next());
            var username = new Username(name);
            var passwordHash = PasswordHash.Generate(plaintextPassword, nextId, username);

            var pendingRestrationRole = _roleRepository.Find(new RolenameSpecification(Role.PENDING_REGISTRATION_ROLENAME));

            var authUser = new AuthUser(nextId, nextUserId, username, passwordHash, new Email(email), new[] { pendingRestrationRole });

            _authUserRepository.Save(authUser);

            _eventQueue.Raise(new AuthUserCreated(authUser));

            return authUser;
        }

        public AuthUser FinishRegistration(AuthUserId id) {
            var registeredUserRole = _roleRepository.Find(new RolenameSpecification(Role.REGISTERED_USER_ROLENAME));
            var authUser = _authUserRepository.FindById(id);

            authUser.Register(registeredUserRole);

            _authUserRepository.Save(authUser);

            return authUser;
        }
    }
}
