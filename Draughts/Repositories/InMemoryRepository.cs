using Draughts.Common.OoConcepts;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories {
    public abstract class InMemoryRepository<T> : IRepository<T> where T : class {
        protected abstract IList<T> GetBaseQuery();
        public T Find(Specification<T> spec) => GetBaseQuery().Single(spec.IsSatisfiedBy);
        public T? FindOrNull(Specification<T> spec) => GetBaseQuery().SingleOrDefault(spec.IsSatisfiedBy);
        public IReadOnlyList<T> List() => GetBaseQuery().ToList().AsReadOnly();
        public IReadOnlyList<T> List<TKey>(Sort<T, TKey> sort) => GetBaseQuery().Sort(sort).ToList().AsReadOnly();
        public IReadOnlyList<T> List(Specification<T> spec) => GetBaseQuery().Where(spec.IsSatisfiedBy).ToList().AsReadOnly();
        public IReadOnlyList<T> List<TKey>(Specification<T> spec, Sort<T, TKey> sort)
            => GetBaseQuery().Where(spec.IsSatisfiedBy).Sort(sort).ToList().AsReadOnly();

        public abstract void Save(T entity);
    }
}
