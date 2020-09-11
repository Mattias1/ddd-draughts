using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using System.Collections.Generic;

namespace Draughts.Application.Auth.Services {
    public interface IAuthService {
        JsonWebToken GenerateJwt(string? name, string? plaintextPassword);
        IReadOnlyList<Permission> PermissionsForJwt(JsonWebToken jwt);
    }
}