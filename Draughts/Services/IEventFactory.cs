using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Services {
    public interface IEventFactory {
        void RaiseAuthUserCreated(AuthUser authUser);
        void RaiseUserCreated(User user);
    }
}