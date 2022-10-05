using Draughts.Common.OoConcepts;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories;

public abstract class BaseRepository<T, TId, TDb> : IRepository<T, TId>
        where T : AggregateRoot<T, TId> where TId : IdValueObject<TId> where TDb : new() {
    protected readonly IRepositoryUnitOfWork UnitOfWork;

    protected BaseRepository(IRepositoryUnitOfWork unitOfWork) {
        UnitOfWork = unitOfWork;
    }

    public long Count() {
        return GetBaseQuery().Select().CountAll().From(TableName).SingleLong();
    }
    public long Count(Specification<T> spec) {
        var query = GetBaseQuery().Select().CountAll().From(TableName);
        ApplySpec(spec, query);
        return query.SingleLong();
    }

    public T FindById(TId id) => Find(new EntityIdSpecification<T, TId>(id));
    public T? FindByIdOrNull(TId id) => FindOrNull(new EntityIdSpecification<T, TId>(id));

    public T Find(Specification<T> spec) => Parse(ApplySpec(spec, SelectAllFromTable()).Single<TDb>());
    public T? FindOrNull(Specification<T> spec) => ParseNullable(ApplySpec(spec, SelectAllFromTable()).SingleOrDefault<TDb>());

    public IReadOnlyList<T> List() {
        var query = SelectAllFromTable();
        return Parse(query.List<TDb>());
    }
    public IReadOnlyList<T> List(Specification<T> spec) {
        var query = SelectAllFromTable();
        ApplySpec(spec, query);
        return Parse(query.List<TDb>());
    }
    public IReadOnlyList<T> List<TKey>(Sort<T, TKey> sort) {
        return Parse(SelectAllFromTable().ApplySort(sort).List<TDb>());
    }
    public IReadOnlyList<T> List<TKey>(Specification<T> spec, Sort<T, TKey> sort) {
        var query = SelectAllFromTable();
        ApplySpec(spec, query);
        return Parse(query.ApplySort(sort).List<TDb>());
    }

    public Pagination<T> Paginate<TKey>(long page, int pageSize, Sort<T, TKey> sort) {
        var p = SelectAllFromTable().ApplySort(sort).Paginate<TDb>(page, pageSize);
        return new Pagination<T>(Parse(p.Results), p.Count, p.PageIndex, p.PageSize);
    }
    public Pagination<T> Paginate<TKey>(long page, int pageSize, Specification<T> spec, Sort<T, TKey> sort) {
        var query = SelectAllFromTable();
        ApplySpec(spec, query);
        var p = query.ApplySort(sort).Paginate<TDb>(page, pageSize);
        return new Pagination<T>(Parse(p.Results), p.Count, p.PageIndex, p.PageSize);
    }

    protected abstract string TableName { get; }
    protected IQueryBuilder SelectAllFromTable() => GetBaseQuery().SelectAllFrom(TableName);
    protected abstract IInitialQueryBuilder GetBaseQuery();

    // Apply the specifications to the query. Can be overridden to add joins if necessary.
    protected virtual IQueryBuilder ApplySpec(Specification<T> spec, IQueryBuilder query) {
        return spec.ApplyQueryBuilder(query);
    }

    protected virtual IReadOnlyList<T> Parse(IReadOnlyList<TDb> results) => results.Select(Parse).ToList().AsReadOnly();
    protected virtual T? ParseNullable(TDb? result) => result is null ? null : Parse(result);
    protected abstract T Parse(TDb result);

    public void Save(T entity) {
        RaiseEvents(entity);
        SaveInternal(entity);
    }
    protected abstract void SaveInternal(T entity);

    protected void RaiseEvents(AggregateRoot<T, TId> aggregateRoot) {
        // TODO: Actually save the events to the database?
        foreach (var evt in aggregateRoot.Events) {
            UnitOfWork.Raise(evt);
        }
        aggregateRoot.ClearEvents();
    }
}
