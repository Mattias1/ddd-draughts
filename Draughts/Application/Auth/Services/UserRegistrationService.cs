using Draughts.Domain.AuthUserContext.Events;
using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.AuthUserContext.Services;
using Draughts.Domain.AuthUserContext.Specifications;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;

namespace Draughts.Application.Auth.Services {
    public class UserRegistrationService : IUserRegistrationService {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IUserRegistrationDomainService _userRegistrationDomainService;
        private readonly IIdGenerator _idGenerator;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserRegistrationService(
                IAuthUserRepository authUserRepository, IUserRegistrationDomainService userRegistrationDomainService,
                IIdGenerator idGenerator, IRoleRepository roleRepository, IUnitOfWork unitOfWork) {
            _authUserRepository = authUserRepository;
            _userRegistrationDomainService = userRegistrationDomainService;
            _idGenerator = idGenerator;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public AuthUser CreateAuthUser(string? name, string? email, string? plaintextPassword) {
            return _unitOfWork.WithAuthUserTransaction(tran => {
                var pendingRegistrationRole = _roleRepository.Find(new RolenameSpecification(Role.PENDING_REGISTRATION_ROLENAME));
                var authUser = _userRegistrationDomainService.CreateAuthUser(_idGenerator.ReservePool(), pendingRegistrationRole,
                    name, email, plaintextPassword);

                _authUserRepository.Save(authUser);

                _unitOfWork.Raise(AuthUserCreated.Factory(authUser));

                return tran.CommitWith(authUser);
            });
        }

        public AuthUser FinishRegistration(UserId id) {
            return _unitOfWork.WithAuthUserTransaction(tran => {
                var registeredUserRole = _roleRepository.Find(new RolenameSpecification(Role.REGISTERED_USER_ROLENAME));
                var pendingRegistrationRole = _roleRepository.Find(new RolenameSpecification(Role.PENDING_REGISTRATION_ROLENAME));
                var authUser = _authUserRepository.FindById(id);

                _userRegistrationDomainService.Register(authUser, registeredUserRole, pendingRegistrationRole);

                _authUserRepository.Save(authUser);

                return tran.CommitWith(authUser);
            });
        }
    }
}
