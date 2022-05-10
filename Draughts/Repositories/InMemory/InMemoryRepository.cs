using Draughts.Common.OoConcepts;
using Draughts.Repositories.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory;

public abstract class InMemoryRepository<T, TId> : IRepository<T, TId>
        where T : AggregateRoot<T, TId> where TId : IdValueObject<TId> {
    protected readonly IRepositoryUnitOfWork UnitOfWork;

    protected InMemoryRepository(IRepositoryUnitOfWork unitOfWork) {
        UnitOfWork = unitOfWork;
    }

    protected abstract IList<T> GetBaseQuery();

    public long Count() => GetBaseQuery().Count;
    public long Count(Specification<T> spec) => GetBaseQuery().Count(spec.IsSatisfiedBy);

    public T FindById(TId id) => Find(new EntityIdSpecification<T, TId>(id));
    public T? FindByIdOrNull(TId id) => FindOrNull(new EntityIdSpecification<T, TId>(id));
    public T Find(Specification<T> spec) => GetBaseQuery().Single(spec.IsSatisfiedBy);
    public T? FindOrNull(Specification<T> spec) => GetBaseQuery().SingleOrDefault(spec.IsSatisfiedBy);

    public IReadOnlyList<T> List() => GetBaseQuery().ToList().AsReadOnly();
    public IReadOnlyList<T> List<TKey>(Sort<T, TKey> sort) => GetBaseQuery().Sort(sort).ToList().AsReadOnly();
    public IReadOnlyList<T> List(Specification<T> spec) => GetBaseQuery().Where(spec.IsSatisfiedBy).ToList().AsReadOnly();
    public IReadOnlyList<T> List<TKey>(Specification<T> spec, Sort<T, TKey> sort)
        => GetBaseQuery().Where(spec.IsSatisfiedBy).Sort(sort).ToList().AsReadOnly();

    public Pagination<T> Paginate<TKey>(long page, int pageSize, Sort<T, TKey> sort) {
        int pageIndex = GetPageIndex(page);
        var allResults = List(sort);
        var results = allResults.Skip(pageIndex * pageSize).Take(pageSize).ToList().AsReadOnly();
        return new Pagination<T>(results, allResults.Count, pageIndex, pageSize);
    }
    public Pagination<T> Paginate<TKey>(long page, int pageSize, Specification<T> spec, Sort<T, TKey> sort) {
        int pageIndex = GetPageIndex(page);
        var allResults = List(spec, sort);
        var results = allResults.Skip(pageIndex * pageSize).Take(pageSize).ToList().AsReadOnly();
        return new Pagination<T>(results, allResults.Count, pageIndex, pageSize);
    }

    private int GetPageIndex(long page) => Math.Max((int)page - 1, 0);

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
