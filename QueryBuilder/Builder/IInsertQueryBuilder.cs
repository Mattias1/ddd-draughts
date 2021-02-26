using System.Collections.Generic;

namespace SqlQueryBuilder.Builder {
    public interface IInsertQueryBuilder : IQueryBuilderBase {
        IInsertQueryBuilder Columns(params string[] columns);
        IInsertQueryBuilder Values(params object?[] parameters);
        IInsertQueryBuilder Values<T>(IEnumerable<T> parameters);

        IInsertQueryBuilder RawInsertColumn(string queryPart, params object?[] parameters);
        ICompleteQueryBuilder RawInsertValue(string queryPart, params object?[] parameters);

        IQueryBuilderBase InsertFrom<T>(params T[] models) where T : notnull;
        IQueryBuilderBase InsertFrom<T>(IEnumerable<T> models) where T : notnull;
        IQueryBuilderBase InsertFromDictionary(IReadOnlyDictionary<string, object?> dictionary);
    }
}
