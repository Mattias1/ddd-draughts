using Draughts.Common.Events;
using Draughts.Common.OoConcepts;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using NodaTime;
using SqlQueryBuilder.Builder;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories;

public sealed class EventsRepository {
    private readonly IRepositoryUnitOfWork _unitOfWork;

    private const string SENT_EVENTS = "sent_events";
    private const string RECEIVED_EVENTS = "received_events";

    public EventsRepository(IRepositoryUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }

    private IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(_unitOfWork.ActiveTransactionDomain());
    private IQueryBuilder SelectAllFromSentEvents() => GetBaseQuery().SelectAllFrom(SENT_EVENTS);

    private IReadOnlyList<DomainEvent> Parse(IReadOnlyList<DbEvent> results) {
        return results.Select(DbEvent.ToDomainModel).ToList().AsReadOnly();
    }

    public Pagination<DomainEvent> PaginateSentEvents<TKey>(long page, int pageSize, Sort<DomainEvent, TKey> sort) {
        var p = SelectAllFromSentEvents().ApplySort(sort).Paginate<DbEvent>(page, pageSize);
        return new Pagination<DomainEvent>(Parse(p.Results), p.Count, p.PageIndex, p.PageSize);
    }

    public Pagination<DomainEvent> PaginateSentEvents<TKey>(long page, int pageSize,
            Specification<DomainEvent> spec, Sort<DomainEvent, TKey> sort) {
        var p = SelectAllFromSentEvents().Specifically(spec).ApplySort(sort).Paginate<DbEvent>(page, pageSize);
        return new Pagination<DomainEvent>(Parse(p.Results), p.Count, p.PageIndex, p.PageSize);
    }

    public static void SaveEvent(DomainEvent evt, IInitialQueryBuilder baseQuery) {
        var obj = DbEvent.FromDomainModel(evt);
        bool eventExists = baseQuery.CountAllFrom(SENT_EVENTS).Where("id").Is(evt.Id).SingleLong() > 0;
        if (eventExists) {
            baseQuery.Update(SENT_EVENTS).SetWithoutIdFrom(obj).Where("id").Is(evt.Id).Execute();
        }
        else {
            baseQuery.InsertInto(SENT_EVENTS).InsertFrom(obj).Execute();
        }
    }

    public bool EventIsReceived(DomainEventId evtId) {
        return GetBaseQuery().CountAllFrom(RECEIVED_EVENTS).Where("id").Is(evtId.Value).SingleLong() > 0;
    }

    public void MarkEventAsReceived(DomainEventId evtId, ZonedDateTime handledAt) {
        var obj = new DbReceivedEvent { Id = evtId.Value, HandledAt = handledAt };
        GetBaseQuery().InsertInto(RECEIVED_EVENTS).InsertFrom(obj).Execute();
    }
}
