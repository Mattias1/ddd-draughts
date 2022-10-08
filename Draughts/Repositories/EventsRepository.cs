using Draughts.Common.Events;
using Draughts.Common.OoConcepts;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories;

public sealed class EventsRepository {
    private readonly IRepositoryUnitOfWork _unitOfWork;

    private string TableName => "sent_events";

    public EventsRepository(IRepositoryUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }

    private IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(_unitOfWork.ActiveTransactionDomain());
    private IQueryBuilder SelectAllFromTable() => GetBaseQuery().SelectAllFrom(TableName);

    private IReadOnlyList<DomainEvent> Parse(IReadOnlyList<DbEvent> results) {
        return results.Select(DbEvent.ToDomainModel).ToList().AsReadOnly();
    }

    public Pagination<DomainEvent> PaginateSentEvents<TKey>(long page, int pageSize, Sort<DomainEvent, TKey> sort) {
        var p = SelectAllFromTable().ApplySort(sort).Paginate<DbEvent>(page, pageSize);
        return new Pagination<DomainEvent>(Parse(p.Results), p.Count, p.PageIndex, p.PageSize);
    }

    public Pagination<DomainEvent> PaginateSentEvents<TKey>(long page, int pageSize,
            Specification<DomainEvent> spec, Sort<DomainEvent, TKey> sort) {
        var p = SelectAllFromTable().Specifically(spec).ApplySort(sort).Paginate<DbEvent>(page, pageSize);
        return new Pagination<DomainEvent>(Parse(p.Results), p.Count, p.PageIndex, p.PageSize);
    }
}
