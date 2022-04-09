using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Repositories;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.Auth.Services;

public sealed class AuthService {
    private readonly IAuthUserRepository _authUserRepository;
    private readonly IClock _clock;
    private readonly IRoleRepository _roleRepository;

    public AuthService(IRoleRepository roleRepository, IClock clock, IAuthUserRepository authUserRepository) {
        _authUserRepository = authUserRepository;
        _clock = clock;
        _roleRepository = roleRepository;
    }

    public JsonWebToken GenerateJwt(string? name, string? plaintextPassword) {
        var usernameOrEmailSpec = new UsernameSpecification(name).Or(new EmailSpecification(name));
        var authUser = _authUserRepository.FindOrNull(usernameOrEmailSpec);
        if (authUser is null || !authUser.PasswordHash.CanLogin(plaintextPassword, authUser.Id, authUser.Username)) {
            throw new ManualValidationException("Incorrect username or password.");
        }

        return JsonWebToken.Generate(authUser, _clock);
    }

    public IReadOnlyList<Permission> PermissionsForJwt(JsonWebToken jwt) {
        return jwt.RoleIds.SelectMany(r => _roleRepository.PermissionsForRole(r)).ToList().AsReadOnly();
    }
}
