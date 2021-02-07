using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Draughts.Common.OoConcepts {
    public abstract class Sort<T, TKey> {
        public bool SortDescending { get; private set; }
        public abstract Expression<Func<T, TKey>> ToExpression();

        public Sort() : this(defaultDescending: false) { }
        public Sort(bool defaultDescending) => SortDescending = defaultDescending;

        public TKey SortKey(T entity) {
            Func<T, TKey> predicate = ToExpression().Compile();
            return predicate(entity);
        }

        public abstract IQueryBuilder ApplyQueryBuilder(IQueryBuilder builder);

        protected IQueryBuilder ApplyColumnSort(IQueryBuilder builder, string column) {
            return SortDescending ? builder.OrderByDesc(column) : builder.OrderByAsc(column);
        }

        public Sort<T, TKey> Asc() {
            SortDescending = false;
            return this;
        }
        public Sort<T, TKey> Desc() {
            SortDescending = true;
            return this;
        }
    }

    public static class SortExtension {
        public static IEnumerable<T> Sort<T, TKey>(this IEnumerable<T> enumerable, Sort<T, TKey> sort) {
            return sort.SortDescending ? enumerable.OrderByDescending(sort.SortKey) : enumerable.OrderBy(sort.SortKey);
        }
    }
}
