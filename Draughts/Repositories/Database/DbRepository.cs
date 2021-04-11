using Draughts.Common.OoConcepts;
using SqlQueryBuilder.Builder;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.Database {
    public abstract class DbRepository<T, TId, TDb> : IRepository<T, TId>
            where T : Entity<T, TId> where TId : IdValueObject<TId> where TDb : new() {
        protected DbRepository() { }

        public long Count() {
            var query = GetBaseQuery().Select().CountAll().From(TableName);
            return query.SingleLong() ?? 0;
        }
        public long Count(Specification<T> spec) {
            var query = GetBaseQuery().Select().CountAll().From(TableName);
            ApplySpec(spec, query);
            return query.SingleLong() ?? 0;
        }

        public T FindById(TId id) => Find(new EntityIdSpecification<T, TId>(id));
        public T? FindByIdOrNull(TId id) => FindOrNull(new EntityIdSpecification<T, TId>(id));

        public T Find(Specification<T> spec) => Parse(ApplySpec(spec, GetBaseSelectQuery()).Single<TDb>());
        public T? FindOrNull(Specification<T> spec) => ParseNullable(ApplySpec(spec, GetBaseSelectQuery()).SingleOrDefault<TDb>());

        public IReadOnlyList<T> List() {
            var query = GetBaseSelectQuery();
            return Parse(query.List<TDb>());
        }
        public IReadOnlyList<T> List<TKey>(Sort<T, TKey> sort) {
            var query = GetBaseSelectQuery();
            sort.ApplyQueryBuilder(query);
            return Parse(query.List<TDb>());
        }
        public IReadOnlyList<T> List(Specification<T> spec) {
            var query = GetBaseSelectQuery();
            ApplySpec(spec, query);
            return Parse(query.List<TDb>());
        }
        public IReadOnlyList<T> List<TKey>(Specification<T> spec, Sort<T, TKey> sort) {
            var query = GetBaseSelectQuery();
            ApplySpec(spec, query);
            sort.ApplyQueryBuilder(query);
            return Parse(query.List<TDb>());
        }

        public Pagination<T> Paginate<TKey>(long page, int pageSize, Sort<T, TKey> sort) {
            var query = GetBaseSelectQuery();
            sort.ApplyQueryBuilder(query);
            var p = query.Paginate<TDb>(page, pageSize);
            return new Pagination<T>(Parse(p.Results), p.Count, p.PageIndex, p.PageSize);
        }
        public Pagination<T> Paginate<TKey>(long page, int pageSize, Specification<T> spec, Sort<T, TKey> sort) {
            var query = GetBaseSelectQuery();
            ApplySpec(spec, query);
            sort.ApplyQueryBuilder(query);
            var p = query.Paginate<TDb>(page, pageSize);
            return new Pagination<T>(Parse(p.Results), p.Count, p.PageIndex, p.PageSize);
        }

        protected abstract string TableName { get; }
        protected IQueryBuilder GetBaseSelectQuery() => GetBaseQuery().SelectAllFrom(TableName);
        protected abstract IInitialQueryBuilder GetBaseQuery();

        protected virtual IQueryBuilder ApplySpec(Specification<T> spec, IQueryBuilder query) {
            return spec.ApplyQueryBuilder(query);
        }

        protected virtual IReadOnlyList<T> Parse(IReadOnlyList<TDb> results) => results.Select(Parse).ToList().AsReadOnly();
        protected virtual T? ParseNullable(TDb? result) => result is null ? null : Parse(result);
        protected abstract T Parse(TDb result);

        public abstract void Save(T entity);
    }
}
