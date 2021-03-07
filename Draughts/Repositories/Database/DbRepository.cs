using Draughts.Common.OoConcepts;
using SqlQueryBuilder.Builder;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.Database {
    public abstract class DbRepository<T, TId, TDb> : IRepository<T, TId>
            where T : Entity<T, TId> where TId : IdValueObject<TId> where TDb : new() {
        protected DbRepository() { }

        public long Count() => GetBaseQuery().Select().CountAll().From(TableName).SingleLong() ?? 0;
        public long Count(Specification<T> spec) => ApplySpec(spec,
            GetBaseQuery().Select().CountAll().From(TableName)).SingleLong() ?? 0;

        public T Find(Specification<T> spec) => Parse(ApplySpec(spec, GetBaseSelectQuery()).Single<TDb>());
        public T? FindOrNull(Specification<T> spec) => ParseNullable(ApplySpec(spec, GetBaseSelectQuery()).SingleOrDefault<TDb>());
        public T FindById(TId id) => Find(new EntityIdSpecification<T, TId>(id));
        public T? FindByIdOrNull(TId id) => FindOrNull(new EntityIdSpecification<T, TId>(id));

        public IReadOnlyList<T> List() => Parse(GetBaseSelectQuery().List<TDb>());
        public IReadOnlyList<T> List<TKey>(Sort<T, TKey> sort) => Parse(sort.ApplyQueryBuilder(GetBaseSelectQuery()).List<TDb>());
        public IReadOnlyList<T> List(Specification<T> spec) => Parse(ApplySpec(spec, GetBaseSelectQuery()).List<TDb>());
        public IReadOnlyList<T> List<TKey>(Specification<T> spec, Sort<T, TKey> sort) {
            var builder = GetBaseSelectQuery();
            ApplySpec(spec, builder);
            sort.ApplyQueryBuilder(builder);
            return Parse(builder.List<TDb>());
        }

        protected abstract string TableName { get; }
        protected IQueryBuilder GetBaseSelectQuery() => GetBaseQuery().SelectAllFrom(TableName);
        protected abstract IInitialQueryBuilder GetBaseQuery();

        protected virtual IQueryBuilder ApplySpec(Specification<T> spec, IQueryBuilder builder) {
            return spec.ApplyQueryBuilder(builder);
        }

        protected virtual IReadOnlyList<T> Parse(IReadOnlyList<TDb> results) => results.Select(Parse).ToList().AsReadOnly();
        protected virtual T? ParseNullable(TDb? result) => result is null ? null : Parse(result);
        protected abstract T Parse(TDb result);

        public abstract void Save(T entity);
    }
}
