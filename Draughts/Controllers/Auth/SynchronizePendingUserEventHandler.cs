using Draughts.Common.Events;
using Draughts.Controllers.Auth.Services;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Repositories.Databases;

namespace Draughts.Controllers.Auth {
    public class SynchronizePendingUserEventHandler : DomainEventHandler<AuthUserCreated> {
        private readonly IUserFactory _userFactory;
        private readonly IUnitOfWork _unitOfWork;

        public SynchronizePendingUserEventHandler(IUserFactory userFactory, IUnitOfWork unitOfWork) : base(AuthUserCreated.TYPE) {
            _userFactory = userFactory;
            _unitOfWork = unitOfWork;
        }

        public override void Handle(AuthUserCreated evt) {
            _unitOfWork.WithTransaction(TransactionDomain.User, tran => {
                _userFactory.CreateUser(evt.AuthUserId, evt.UserId, evt.Username);

                tran.Commit();
            });
        }
    }
}
