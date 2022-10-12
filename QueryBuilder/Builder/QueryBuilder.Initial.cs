using SqlQueryBuilder.Model;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : IInitialQueryBuilder {
    public IQueryBuilder SelectAllFrom(string table) => SelectAll(table).From(table);
    public IQueryBuilder SelectAllFromAs(string table, string alias) => SelectAll(alias).FromAs(table, alias);
    public IQueryBuilder CountAllFrom(string table) => CountAll().From(table);
    public ISelectQueryBuilder SelectAll(string table) => Select(table + ".*");
    public ISelectQueryBuilder SelectAll() => Select("*");

    public ISelectQueryBuilder SelectDistinct() {
        _query.Distinct = true;
        return Select();
    }

    public ISelectQueryBuilder Select(params string[] columns) {
        foreach (string c in columns) {
            _query.SelectColumns.Add(new Column(c));
        }
        return Select();
    }
    public ISelectQueryBuilder Select() => this;

    public IInsertQueryBuilder InsertInto(string table) {
        _query.InsertTable = new Table(table);
        return this;
    }

    public IUpdateQueryBuilder Update(string table) {
        _query.UpdateTable = new Table(table);
        return this;
    }

    public IQueryBuilder DeleteFrom(string table) {
        _query.DeleteTable = new Table(table);
        return this;
    }
    public IQueryBuilder DeleteFromAs(string table, string alias) {
        _query.DeleteTable = new Table(table, alias);
        return this;
    }

    public ICompleteQueryBuilder Raw(string query, params object?[] parameters) {
        _query.RawQueryParts.Add(new RawQueryPart(query, parameters));
        return this;
    }
}
