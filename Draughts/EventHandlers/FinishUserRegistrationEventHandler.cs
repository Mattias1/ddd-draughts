using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Repositories.Databases;
using Draughts.Services;

namespace Draughts.EventHandlers {
    public class FinishUserRegistrationEventHandler : DomainEventHandler<UserCreated> {
        private readonly IAuthUserFactory _authUserFactory;
        private readonly IUnitOfWork _unitOfWork;

        public FinishUserRegistrationEventHandler(IAuthUserFactory authUserFactory, IUnitOfWork unitOfWork) : base(UserCreated.TYPE) {
            _authUserFactory = authUserFactory;
            _unitOfWork = unitOfWork;
        }

        public override void Handle(UserCreated evt) {
            _unitOfWork.WithTransaction(TransactionDomain.AuthUser, tran => {
                _authUserFactory.FinishRegistration(evt.AuthUserId);

                tran.Commit();
            });
        }
    }
}
