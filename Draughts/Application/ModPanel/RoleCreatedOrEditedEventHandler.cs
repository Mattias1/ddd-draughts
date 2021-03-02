using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Repositories.Transaction;
using Draughts.Repositories;
using Draughts.Domain.AuthUserAggregate.Models;
using NodaTime;
using System;

namespace Draughts.Application.Auth {
    public class RoleCreatedOrEditedEventHandler : DomainEventHandler {
        private readonly IAdminLogRepository _adminLogRepository;
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IClock _clock;
        private readonly IIdGenerator _idGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public RoleCreatedOrEditedEventHandler(
                IAdminLogRepository adminLogRepository, IAuthUserRepository authUserRepository,
                IClock clock, IIdGenerator idGenerator, IUnitOfWork unitOfWork)
                : base(RoleCreated.TYPE, RoleEdited.TYPE) {
            _adminLogRepository = adminLogRepository;
            _authUserRepository = authUserRepository;
            _clock = clock;
            _idGenerator = idGenerator;
            _unitOfWork = unitOfWork;
        }

        public override void Handle(DomainEvent evt) {
            if (evt is RoleCreated roleCreatedEvent) {
                Handle(roleCreatedEvent);
                return;
            }
            if (evt is RoleEdited roleEditedEvent) {
                Handle(roleEditedEvent);
                return;
            }
            throw new InvalidOperationException("Invalid role event type.");
        }

        public void Handle(RoleCreated evt) {
            _unitOfWork.WithAuthUserTransaction(tran => {
                var authuser = _authUserRepository.FindById(evt.CreatedBy);
                var adminLog = AdminLog.CreateRoleLog(_idGenerator.ReservePool(), _clock, authuser, evt.RoleId, evt.Rolename);
                _adminLogRepository.Save(adminLog);

                tran.Commit();
            });
        }

        public void Handle(RoleEdited evt) {
            _unitOfWork.WithAuthUserTransaction(tran => {
                var authuser = _authUserRepository.FindById(evt.EditedBy);
                var adminLog = AdminLog.EditRoleLog(_idGenerator.ReservePool(), _clock, authuser, evt.RoleId, evt.Rolename);
                _adminLogRepository.Save(adminLog);

                tran.Commit();
            });
        }
    }
}
