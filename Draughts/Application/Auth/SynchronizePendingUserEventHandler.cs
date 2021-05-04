using Draughts.Common.Events;
using Draughts.Application.Auth.Services;
using Draughts.Domain.AuthUserContext.Events;
using Draughts.Repositories.Transaction;

namespace Draughts.Application.Auth {
    public class SynchronizePendingUserEventHandler : DomainEventHandler<AuthUserCreated> {
        private readonly IUserFactory _userFactory;
        private readonly IUnitOfWork _unitOfWork;

        public SynchronizePendingUserEventHandler(IUserFactory userFactory, IUnitOfWork unitOfWork) {
            _userFactory = userFactory;
            _unitOfWork = unitOfWork;
        }

        public override void Handle(AuthUserCreated evt) {
            _unitOfWork.WithTransaction(TransactionDomain.User, tran => {
                _userFactory.CreateUser(evt.UserId, evt.Username);

                tran.Commit();
            });
        }
    }
}
