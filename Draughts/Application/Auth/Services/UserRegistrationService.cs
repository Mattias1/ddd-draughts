using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Services;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Application.Auth.Services;

public class UserRegistrationService {
    private readonly IAuthUserRepository _authUserRepository;
    private readonly IClock _clock;
    private readonly IUserRepository _userRepository;
    private readonly IUserRegistrationDomainService _userRegistrationDomainService;
    private readonly IIdGenerator _idGenerator;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserRegistrationService(
            IAuthUserRepository authUserRepository, IClock clock, IIdGenerator idGenerator,
            IRoleRepository roleRepository, IUnitOfWork unitOfWork, IUserRepository userRepository,
            IUserRegistrationDomainService userRegistrationDomainService) {
        _authUserRepository = authUserRepository;
        _clock = clock;
        _idGenerator = idGenerator;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _userRegistrationDomainService = userRegistrationDomainService;
    }

    public AuthUser CreateAuthUser(string? name, string? email, string? plaintextPassword) {
        return _unitOfWork.WithAuthTransaction(tran => {
            var pendingRegistrationRole = _roleRepository.Find(new RolenameSpecification(Role.PENDING_REGISTRATION_ROLENAME));

            ValidateUsernameAndEmailUniqueness(name, email);

            var authUser = _userRegistrationDomainService.CreateAuthUser(_idGenerator.ReservePool(),
                pendingRegistrationRole, name, email, plaintextPassword);

            _authUserRepository.Save(authUser);

            _unitOfWork.Raise(AuthUserCreated.Factory(authUser));

            return authUser;
        });
    }

    private void ValidateUsernameAndEmailUniqueness(string? name, string? email) {
        var usernameOrEmailSpec = new UsernameSpecification(name).Or(new EmailSpecification(email));
        if (_authUserRepository.Count(usernameOrEmailSpec) > 0) {
            throw new ManualValidationException("This username or email address is already taken.");
        }
    }

    public User CreateUser(UserId userId, Username username) {
        return _unitOfWork.WithTransaction(TransactionDomain.User, tran => {
            var user = User.BuildNew(userId, username, _clock.UtcNow());
            _userRepository.Save(user);

            _unitOfWork.Raise(UserCreated.Factory(user));

            return user;
        });
    }

    public AuthUser FinishRegistration(UserId id) {
        return _unitOfWork.WithAuthTransaction(tran => {
            var registeredUserRole = _roleRepository.Find(new RolenameSpecification(Role.REGISTERED_USER_ROLENAME));
            var pendingRegistrationRole = _roleRepository.Find(new RolenameSpecification(Role.PENDING_REGISTRATION_ROLENAME));
            var authUser = _authUserRepository.FindById(id);

            _userRegistrationDomainService.Register(authUser, registeredUserRole, pendingRegistrationRole);

            _authUserRepository.Save(authUser);

            return authUser;
        });
    }
}
