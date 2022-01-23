using System.Collections.Generic;

namespace SqlQueryBuilder.Builder;

public interface IInsertQueryBuilder : IQueryBuilderResult {
    IInsertQueryBuilder Columns(params string[] columns);
    IInsertQueryBuilder Values(params object?[] parameters);
    IInsertQueryBuilder Values<T>(IEnumerable<T> parameters);

    IInsertQueryBuilder RawInsertColumn(string queryPart, params object?[] parameters);
    ICompleteQueryBuilder RawInsertValue(string queryPart, params object?[] parameters);

    IQueryBuilderResult InsertFrom<T>(params T[] models) where T : notnull;
    IQueryBuilderResult InsertFrom<T>(IEnumerable<T> models) where T : notnull;
    IQueryBuilderResult InsertFromDictionary(IReadOnlyDictionary<string, object?> dictionary);

    ISelectQueryBuilder Select();
}
