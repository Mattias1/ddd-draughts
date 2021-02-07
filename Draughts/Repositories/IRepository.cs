using Draughts.Common.OoConcepts;
using System.Collections.Generic;

namespace Draughts.Repositories {
    public interface IRepository<T> where T : class {
        long Count();
        long Count(Specification<T> spec);
        T Find(Specification<T> spec);
        T? FindOrNull(Specification<T> spec);
        IReadOnlyList<T> List();
        IReadOnlyList<T> List(Specification<T> spec);
        IReadOnlyList<T> List<TKey>(Sort<T, TKey> sort);
        IReadOnlyList<T> List<TKey>(Specification<T> spec, Sort<T, TKey> sort);
        void Save(T entity);
    }
}