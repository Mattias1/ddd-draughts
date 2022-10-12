using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using NodaTime;
using System;

namespace Draughts.Application.Auth;

public sealed class ModPanelRoleEventHandler : DomainEventHandler {
    private static readonly string[] EVENT_TYPES = new string[] {
        RoleCreated.TYPE, RoleEdited.TYPE, RoleDeleted.TYPE, UserGainedRole.TYPE, UserLostRole.TYPE
    };

    private readonly AdminLogRepository _adminLogRepository;
    private readonly AuthUserRepository _authUserRepository;
    private readonly IClock _clock;
    private readonly IIdGenerator _idGenerator;

    public ModPanelRoleEventHandler(
            AdminLogRepository adminLogRepository, AuthUserRepository authUserRepository,
            EventsRepository eventsRepository, IClock clock, IIdGenerator idGenerator, IUnitOfWork unitOfWork)
            : base(clock, eventsRepository, unitOfWork, EVENT_TYPES) {
        _adminLogRepository = adminLogRepository;
        _authUserRepository = authUserRepository;
        _clock = clock;
        _idGenerator = idGenerator;
    }

    public override void Handle(DomainEvent evt) {
        switch (evt) {
            case RoleCreated roleCreated:
                Handle(roleCreated);
                break;
            case RoleEdited roleEdited:
                Handle(roleEdited);
                break;
            case RoleDeleted roleDeleted:
                Handle(roleDeleted);
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
        HandleWithTransaction(TransactionDomain.Auth, evt, tran => {
            var authUser = _authUserRepository.FindById(evt.CreatedBy);
            var adminLog = AdminLog.CreateRoleLog(_idGenerator.ReservePool(), _clock, authUser, evt.RoleId, evt.Rolename);
            _adminLogRepository.Save(adminLog);
        });
    }

    public void Handle(RoleEdited evt) {
        HandleWithTransaction(TransactionDomain.Auth, evt, tran => {
            var authUser = _authUserRepository.FindById(evt.EditedBy);
            var adminLog = AdminLog.EditRoleLog(_idGenerator.ReservePool(), _clock, authUser, evt.RoleId, evt.Rolename);
            _adminLogRepository.Save(adminLog);
        });
    }

    public void Handle(RoleDeleted evt) {
        HandleWithTransaction(TransactionDomain.Auth, evt, tran => {
            var authUser = _authUserRepository.FindById(evt.DeletedBy);
            var adminLog = AdminLog.RoleDeletedLog(_idGenerator.ReservePool(), _clock, authUser,
                evt.RoleId, evt.Rolename);
            _adminLogRepository.Save(adminLog);
        });
    }

    public void Handle(UserGainedRole evt) {
        HandleWithTransaction(TransactionDomain.Auth, evt, tran => {
            var authUser = _authUserRepository.FindById(evt.AssignedBy);
            var adminLog = AdminLog.RoleGainedLog(_idGenerator.ReservePool(), _clock, authUser,
                evt.RoleId, evt.Rolename, evt.UserId, evt.Username);
            _adminLogRepository.Save(adminLog);
        });
    }

    public void Handle(UserLostRole evt) {
        HandleWithTransaction(TransactionDomain.Auth, evt, tran => {
            var authUser = _authUserRepository.FindById(evt.RemovedBy);
            var adminLog = AdminLog.RoleLostLog(_idGenerator.ReservePool(), _clock, authUser,
                evt.RoleId, evt.Rolename, evt.UserId, evt.Username);
            _adminLogRepository.Save(adminLog);
        });
    }
}
