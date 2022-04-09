using SqlQueryBuilder.Model;
using System.Collections.Generic;
using static SqlQueryBuilder.Model.Where;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : IWhereQueryBuilder {
    private (WhereType type, string column, List<IWhere> forest)? _preparedLeaf;

    public IQueryBuilder Where(SubWhereFunc queryFunc) => And(queryFunc);
    public IQueryBuilder And(SubWhereFunc queryFunc) => RecursiveWhere(WhereType.And, queryFunc);

    public IComparisonQueryBuilder Where(string column) => And(column);
    public IComparisonQueryBuilder And(string column) => PrepareWhereLeaf(WhereType.And, column);

    public IQueryBuilder OrWhere(SubWhereFunc queryFunc) => Or(queryFunc);
    public IQueryBuilder Or(SubWhereFunc queryFunc) => RecursiveWhere(WhereType.Or, queryFunc);

    public IComparisonQueryBuilder OrWhere(string column) => Or(column);
    public IComparisonQueryBuilder Or(string column) => PrepareWhereLeaf(WhereType.Or, column);

    public IQueryBuilder WhereNot(SubWhereFunc queryFunc) => AndNot(queryFunc);
    public IQueryBuilder AndNot(SubWhereFunc queryFunc) => RecursiveWhere(WhereType.AndNot, queryFunc);
    public IQueryBuilder OrWhereNot(SubWhereFunc queryFunc) => OrNot(queryFunc);
    public IQueryBuilder OrNot(SubWhereFunc queryFunc) => RecursiveWhere(WhereType.OrNot, queryFunc);

    private IQueryBuilder RecursiveWhere(WhereType type, SubWhereFunc queryFunc) {
        var queryBuilder = new QueryBuilder(_options);
        queryFunc(queryBuilder);
        _query.WhereForest.Add(new Where(type, queryBuilder._query.WhereForest));
        return this;
    }

    private IComparisonQueryBuilder PrepareWhereLeaf(WhereType type, string column) {
        _preparedLeaf = (type, column, _query.WhereForest);
        return this;
    }

    public IQueryBuilder WhereExists(SubQueryFunc queryFunc) => SubqueryWhere("exists", queryFunc);
    public IQueryBuilder WhereNotExists(SubQueryFunc queryFunc) => SubqueryWhere("not exists", queryFunc);

    private IQueryBuilder SubqueryWhere(string @operator, SubQueryFunc queryFunc) {
        var queryBuilder = new QueryBuilder(_options);
        queryFunc(queryBuilder);
        _query.WhereForest.Add(new WhereSubQuery(WhereType.And, null, @operator, queryBuilder));
        return this;
    }

    public IQueryBuilder RawWhere(string queryPart, params object?[] parameters) {
        _query.WhereForest.Add(new RawQueryPart(queryPart, parameters));
        return this;
    }

    public IQueryBuilder WithoutWhere() {
        _explicitlyWithoutWhere = true;
        return this;
    }
}
