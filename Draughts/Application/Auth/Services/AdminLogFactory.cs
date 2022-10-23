using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using NodaTime;
using System.Linq;

namespace Draughts.Application.Auth.Services;

/// <summary>
/// A factory to log admin actions. This starts its own transaction (unless in the auth context)
/// and should be called before the action is taken to ensure everything is always logged.
/// </summary>
public sealed class AdminLogFactory {
    private readonly AdminLogRepository _adminLogRepository;
    private readonly IClock _clock;
    private readonly IIdGenerator _idGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public AdminLogFactory(AdminLogRepository adminLogRepository, IClock clock, IIdGenerator idGenerator,
            IUnitOfWork unitOfWork) {
        _adminLogRepository = adminLogRepository;
        _clock = clock;
        _idGenerator = idGenerator;
        _unitOfWork = unitOfWork;
    }

    public AdminLog LogCreateRole(UserId actorId, Username actorName, RoleId roleId, string rolename) {
        return CreateLog("role.create", actorId, actorName, roleId, rolename);
    }

    public AdminLog LogEditRole(UserId actorId, Username actorName, RoleId roleId, string rolename) {
        return CreateLog("role.edit", actorId, actorName, roleId, rolename);
    }

    public AdminLog LogGainRole(UserId actorId, Username actorName,
            RoleId roleId, string rolename, UserId userId, Username username) {
        return CreateLog("role.gain", actorId, actorName, roleId, rolename, userId, username);
    }

    public AdminLog LogLoseRole(UserId actorId, Username actorName,
            RoleId roleId, string rolename, UserId userId, Username username) {
        return CreateLog("role.lose", actorId, actorName, roleId, rolename, userId, username);
    }

    public AdminLog LogDeleteRole(UserId actorId, Username actorName, RoleId roleId, string rolename) {
        return CreateLog("role.delete", actorId, actorName, roleId, rolename);
    }

    public AdminLog LogChangeTurnTime(UserId actorId, Username actorName,
            GameId gameId, int turnTimeInSeconds, bool forAllFutureTurns) {
        return CreateLogWithTransaction("game.turntimechange", actorId, actorName,
            gameId, turnTimeInSeconds, forAllFutureTurns);
    }

    public AdminLog LogSyncEventQueueStatus(UserId actorId, Username actorName) {
        return CreateLogWithTransaction("events.sync", actorId, actorName);
    }

    public AdminLog LogDispatchEventQueue(UserId actorId, Username actorName) {
        return CreateLogWithTransaction("events.dispatch", actorId, actorName);
    }

    private AdminLog CreateLogWithTransaction(string type, UserId actorId, Username name, params object[] parameters) {
        return _unitOfWork.WithAuthTransaction(tran => {
            return CreateLog(type, actorId, name, parameters);
        });
    }

    private AdminLog CreateLog(string type, UserId actorId, Username name, params object[] parameters) {
        var logId = new AdminLogId(_idGenerator.ReservePool().Next());
        var parameterList = parameters.Select(o => o.ToString() ?? "").ToList().AsReadOnly();
        var result = new AdminLog(logId, type, parameterList, actorId, name, _clock.UtcNow());

        _adminLogRepository.Save(result);
        return result;
    }
}
