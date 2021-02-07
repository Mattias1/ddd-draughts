using System.Collections.Generic;

namespace SqlQueryBuilder.Builder {
    public interface IInsertQueryBuilder : IQueryBuilderBase {
        IInsertQueryBuilder Columns(params string[] columns);
        IQueryBuilderBase Values(params object?[] parameters);
        IQueryBuilderBase Values<T>(IEnumerable<T> parameters);

        IInsertQueryBuilder RawInsertColumn(string queryPart, params object?[] parameters);
        ICompleteQueryBuilder RawInsertValue(string queryPart, params object?[] parameters);

        IQueryBuilderBase InsertFrom<T>(T model) where T : notnull;
        IQueryBuilderBase InsertFromDictionary(IReadOnlyDictionary<string, object?> dictionary);
    }
}