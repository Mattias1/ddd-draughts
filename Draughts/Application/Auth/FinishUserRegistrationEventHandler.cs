using Draughts.Common.Events;
using Draughts.Application.Auth.Services;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Repositories.Transaction;

namespace Draughts.Application.Auth {
    public class FinishUserRegistrationEventHandler : DomainEventHandler<UserCreated> {
        private readonly IAuthUserFactory _authUserFactory;
        private readonly IUnitOfWork _unitOfWork;

        public FinishUserRegistrationEventHandler(IAuthUserFactory authUserFactory, IUnitOfWork unitOfWork) {
            _authUserFactory = authUserFactory;
            _unitOfWork = unitOfWork;
        }

        public override void Handle(UserCreated evt) {
            _unitOfWork.WithTransaction(TransactionDomain.AuthUser, tran => {
                _authUserFactory.FinishRegistration(evt.UserId);

                tran.Commit();
            });
        }
    }
}
