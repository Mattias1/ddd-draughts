using Draughts.Common.Events;
using Draughts.Common.Events.Specifications;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using NodaTime;
using System;
using System.Linq;

namespace Draughts.Application.ModPanel.Services;

public sealed class SystemEventQueueService {
    private readonly AdminLogRepository _adminLogRepository;
    private readonly EventDispatcher _eventDispatcher;
    private readonly EventsRepository _eventsRepository;
    private readonly IClock _clock;
    private readonly IIdGenerator _idGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public SystemEventQueueService(AdminLogRepository adminLogRepository, EventDispatcher eventDispatcher,
            EventsRepository eventsRepository, IClock clock, IIdGenerator idGenerator, IUnitOfWork unitOfWork) {
        _adminLogRepository = adminLogRepository;
        _eventDispatcher = eventDispatcher;
        _eventsRepository = eventsRepository;
        _clock = clock;
        _idGenerator = idGenerator;
        _unitOfWork = unitOfWork;
    }

    public (Pagination<DomainEvent> AuthEvents, Pagination<DomainEvent> UserEvents, Pagination<DomainEvent> GameEvents)
            ViewEventQueues(int page, int pageSize) {
        var authEvents = WithTran(TransactionDomain.Auth, r => r.PaginateSentEvents(page, pageSize, new EventIdSort()));
        var userEvents = WithTran(TransactionDomain.User, r => r.PaginateSentEvents(page, pageSize, new EventIdSort()));
        var gameEvents = WithTran(TransactionDomain.Game, r => r.PaginateSentEvents(page, pageSize, new EventIdSort()));
        return (authEvents, userEvents, gameEvents);
    }

    public void SyncEventQueueStatus(UserId currentUserId, Username currentUsername) {
        _unitOfWork.WithAuthTransaction(tran => {
            var adminLog = AdminLog.SyncingEventQueueStatus(_idGenerator.ReservePool(), _clock,
                currentUserId, currentUsername);
            _adminLogRepository.Save(adminLog);
        });

        var allDomains = new [] { TransactionDomain.Auth, TransactionDomain.User, TransactionDomain.Game };
        foreach (var sentDomain in allDomains) {
            var unhandledEventIds = WithTran(sentDomain, r => r.ListUnhandledEventIds(limit: 200));
            var receivedEvents = allDomains.SelectMany(d => WithTran(d, r => r.ListReceivedEventsForIds(unhandledEventIds)));
            foreach (var receivedEvent in receivedEvents) {
                WithTran(sentDomain, r => r.MarkEventAsReceived(receivedEvent.Id, receivedEvent.HandledAt));
            }
        }
    }

    public void RedispatchEventQueue(UserId currentUserId, Username currentUsername) {
        _unitOfWork.WithAuthTransaction(tran => {
            var adminLog = AdminLog.DispatchEventQueue(_idGenerator.ReservePool(), _clock,
                currentUserId, currentUsername);
            _adminLogRepository.Save(adminLog);
        });

        var allDomains = new [] { TransactionDomain.Auth, TransactionDomain.User, TransactionDomain.Game };
        foreach (var domain in allDomains) {
            var unhandledEvents = WithTran(domain, r => r.ListUnhandledEvents(limit: 200));
            _eventDispatcher.DispatchAll(unhandledEvents);
        }
    }

    private T WithTran<T>(TransactionDomain transactionDomain, Func<EventsRepository, T> repositoryFunc) {
        return _unitOfWork.WithTransaction(transactionDomain, tran => repositoryFunc(_eventsRepository));
    }
    private void WithTran(TransactionDomain transactionDomain, Action<EventsRepository> repositoryFunc) {
        _unitOfWork.WithTransaction(transactionDomain, tran => repositoryFunc(_eventsRepository));
    }
}
