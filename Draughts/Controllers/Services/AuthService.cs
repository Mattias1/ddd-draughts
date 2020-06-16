using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Repositories;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Controllers.Services {
    public class AuthService : IAuthService {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IClock _clock;
        private readonly IRoleRepository _roleRepository;

        public AuthService(IRoleRepository roleRepository, IClock clock, IAuthUserRepository authUserRepository) {
            _authUserRepository = authUserRepository;
            _clock = clock;
            _roleRepository = roleRepository;
        }

        // TODO: Should this be here? Or should this be in a domain service?
        public JsonWebToken GenerateJwt(string? name, string? plaintextPassword) {
            var usernameOrEmailSpec = new UsernameSpecification(name).Or(new EmailSpecification(plaintextPassword));
            var authUser = _authUserRepository.FindOrNull(usernameOrEmailSpec);
            if (authUser is null || !authUser.PasswordHash.CanLogin(plaintextPassword, authUser.Id, authUser.Username)) {
                throw new ManualValidationException("Incorrect username or password.");
            }

            return JsonWebToken.Generate(authUser, _clock);
        }

        // TODO: Should this be here? Or should this be in a domain service?
        public IReadOnlyList<Permission> PermissionsForJwt(JsonWebToken jwt) {
            return jwt.RoleIds.SelectMany(r => _roleRepository.PermissionsForRole(r)).ToList().AsReadOnly();
        }
    }
}
