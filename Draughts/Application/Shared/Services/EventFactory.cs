using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Application.Shared.Services {
    public class EventFactory : IEventFactory {
        private readonly IClock _clock;
        private readonly IIdGenerator _idGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public EventFactory(IClock clock, IIdGenerator idGenerator, IUnitOfWork unitOfWork) {
            _clock = clock;
            _idGenerator = idGenerator;
            _unitOfWork = unitOfWork;
        }

        public void RaiseAuthUserCreated(AuthUser authUser) {
            var evt = new AuthUserCreated(authUser, NextId(), Now());
            _unitOfWork.Raise(evt);
        }

        public void RaiseUserCreated(User user) {
            var evt = new UserCreated(user, NextId(), Now());
            _unitOfWork.Raise(evt);
        }

        private ZonedDateTime Now() => _clock.GetCurrentInstant().InUtc();
        private DomainEventId NextId() => new DomainEventId(_idGenerator.ReservePool(1, 0, 0).Next());
    }
}
