using Draughts.Common.OoConcepts;
using System.Collections.Generic;

namespace Draughts.Repositories;

public interface IRepositoryWithoutPagination<T, TId> where T : class {
    long Count();
    long Count(Specification<T> spec);
    T FindById(TId id);
    T? FindByIdOrNull(TId id);
    T Find(Specification<T> spec);
    T? FindOrNull(Specification<T> spec);
    IReadOnlyList<T> List();
    IReadOnlyList<T> List(Specification<T> spec);
    IReadOnlyList<T> List<TKey>(Sort<T, TKey> sort);
    IReadOnlyList<T> List<TKey>(Specification<T> spec, Sort<T, TKey> sort);
    void Save(T entity);
}

public interface IRepository<T, TId> : IRepositoryWithoutPagination<T, TId> where T : class {
    Pagination<T> Paginate<TKey>(long page, int pageSize, Sort<T, TKey> sort);
    Pagination<T> Paginate<TKey>(long page, int pageSize, Specification<T> spec, Sort<T, TKey> sort);
}
