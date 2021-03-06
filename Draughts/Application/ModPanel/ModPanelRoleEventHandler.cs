using Draughts.Common.Events;
using Draughts.Domain.AuthUserContext.Events;
using Draughts.Repositories.Transaction;
using Draughts.Repositories;
using Draughts.Domain.AuthUserContext.Models;
using NodaTime;
using System;

namespace Draughts.Application.Auth {
    public class ModPanelRoleEventHandler : DomainEventHandler {
        private readonly IAdminLogRepository _adminLogRepository;
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IClock _clock;
        private readonly IIdGenerator _idGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public ModPanelRoleEventHandler(
                IAdminLogRepository adminLogRepository, IAuthUserRepository authUserRepository,
                IClock clock, IIdGenerator idGenerator, IUnitOfWork unitOfWork)
                : base(RoleCreated.TYPE, RoleEdited.TYPE, UserGainedRole.TYPE, UserLostRole.TYPE) {
            _adminLogRepository = adminLogRepository;
            _authUserRepository = authUserRepository;
            _clock = clock;
            _idGenerator = idGenerator;
            _unitOfWork = unitOfWork;
        }

        public override void Handle(DomainEvent evt) {
            switch (evt) {
                case RoleCreated roleCreated:
                    Handle(roleCreated);
                    break;
                case RoleEdited roleEdited:
                    Handle(roleEdited);
                    break;
                case UserGainedRole roleGained:
                    Handle(roleGained);
                    break;
                case UserLostRole roleLost:
                    Handle(roleLost);
                    break;
                default:
                    throw new InvalidOperationException("Invalid role event type.");
            }
        }

        public void Handle(RoleCreated evt) {
            _unitOfWork.WithAuthUserTransaction(tran => {
                var authUser = _authUserRepository.FindById(evt.CreatedBy);
                var adminLog = AdminLog.CreateRoleLog(_idGenerator.ReservePool(), _clock, authUser, evt.RoleId, evt.Rolename);
                _adminLogRepository.Save(adminLog);

                tran.Commit();
            });
        }

        public void Handle(RoleEdited evt) {
            _unitOfWork.WithAuthUserTransaction(tran => {
                var authUser = _authUserRepository.FindById(evt.EditedBy);
                var adminLog = AdminLog.EditRoleLog(_idGenerator.ReservePool(), _clock, authUser, evt.RoleId, evt.Rolename);
                _adminLogRepository.Save(adminLog);

                tran.Commit();
            });
        }

        public void Handle(UserGainedRole evt) {
            _unitOfWork.WithAuthUserTransaction(tran => {
                var authUser = _authUserRepository.FindById(evt.AssignedBy);
                var adminLog = AdminLog.RoleGainedLog(_idGenerator.ReservePool(), _clock, authUser,
                    evt.RoleId, evt.Rolename, evt.UserId, evt.Username);
                _adminLogRepository.Save(adminLog);

                tran.Commit();
            });
        }

        public void Handle(UserLostRole evt) {
            _unitOfWork.WithAuthUserTransaction(tran => {
                var authUser = _authUserRepository.FindById(evt.RemovedBy);
                var adminLog = AdminLog.RoleLostLog(_idGenerator.ReservePool(), _clock, authUser,
                    evt.RoleId, evt.Rolename, evt.UserId, evt.Username);
                _adminLogRepository.Save(adminLog);

                tran.Commit();
            });
        }
    }
}
