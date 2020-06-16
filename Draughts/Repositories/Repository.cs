using Draughts.Common;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories {
    public abstract class Repository<T> : IRepository<T> where T : class {
        protected abstract IEnumerable<T> BaseQuery { get; }

        public T Find(Specification<T> spec) => BaseQuery.Single(spec.IsSatisfiedBy);
        public T? FindOrNull(Specification<T> spec) => BaseQuery.SingleOrDefault(spec.IsSatisfiedBy);
        public IReadOnlyList<T> List() => BaseQuery.ToList().AsReadOnly();
        public IReadOnlyList<T> List<TKey>(Sort<T, TKey> sort) => BaseQuery.Sort(sort).ToList().AsReadOnly();
        public IReadOnlyList<T> List(Specification<T> spec) => BaseQuery.Where(spec.IsSatisfiedBy).ToList().AsReadOnly();
        public IReadOnlyList<T> List<TKey>(Specification<T> spec, Sort<T, TKey> sort)
            => BaseQuery.Where(spec.IsSatisfiedBy).Sort(sort).ToList().AsReadOnly();

        public abstract void Save(T entity);
    }
}
