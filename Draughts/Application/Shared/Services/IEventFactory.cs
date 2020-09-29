using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Application.Shared.Services {
    public interface IEventFactory {
        void RaiseAuthUserCreated(AuthUser authUser);
        void RaiseUserCreated(User user);
    }
}