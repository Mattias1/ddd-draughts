using Draughts.Common.OoConcepts;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public abstract class InMemoryRepository<T, TId> : IRepository<T, TId>
            where T : Entity<T, TId> where TId : IdValueObject<TId> {
        protected abstract IList<T> GetBaseQuery();

        public long Count() => GetBaseQuery().Count;
        public long Count(Specification<T> spec) => GetBaseQuery().Count(spec.IsSatisfiedBy);

        public T Find(Specification<T> spec) => GetBaseQuery().Single(spec.IsSatisfiedBy);
        public T? FindOrNull(Specification<T> spec) => GetBaseQuery().SingleOrDefault(spec.IsSatisfiedBy);
        public T FindById(TId id) => Find(new EntityIdSpecification<T, TId>(id));
        public T? FindByIdOrNull(TId id) => FindOrNull(new EntityIdSpecification<T, TId>(id));

        public IReadOnlyList<T> List() => GetBaseQuery().ToList().AsReadOnly();
        public IReadOnlyList<T> List<TKey>(Sort<T, TKey> sort) => GetBaseQuery().Sort(sort).ToList().AsReadOnly();
        public IReadOnlyList<T> List(Specification<T> spec) => GetBaseQuery().Where(spec.IsSatisfiedBy).ToList().AsReadOnly();
        public IReadOnlyList<T> List<TKey>(Specification<T> spec, Sort<T, TKey> sort)
            => GetBaseQuery().Where(spec.IsSatisfiedBy).Sort(sort).ToList().AsReadOnly();

        public abstract void Save(T entity);
    }
}
