using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Common;
using SqlQueryBuilder.Exceptions;
using SqlQueryBuilder.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlQueryBuilder.Model;

internal sealed class Query {
    public QueryBuilderOptions Options { get; }

    public List<IQueryPart> RawQueryParts { get; } = new List<IQueryPart>();

    public ITable? InsertTable { get; set; }
    public List<IColumn> InsertColumns { get; } = new List<IColumn>();
    public List<IInsertValue> InsertValues { get; } = new List<IInsertValue>();

    public ITable? UpdateTable { get; set; }
    public List<ISetColumn> UpdateValues { get; } = new List<ISetColumn>();

    public ITable? DeleteTable { get; set; }

    public List<IColumn> SelectColumns { get; } = new List<IColumn>();
    public bool Distinct { get; set; } = false;

    public List<ITable> SelectFrom { get; } = new List<ITable>();
    public List<IJoin> Joins { get; } = new List<IJoin>();
    public List<IWhere> WhereForest { get; } = new List<IWhere>();
    public List<IColumn> GroupByColumns { get; } = new List<IColumn>();
    public List<IWhere> HavingForest { get; } = new List<IWhere>();
    public List<IOrderBy> OrderByList { get; } = new List<IOrderBy>();

    public List<ILimit> Limits { get; set; } = new List<ILimit>();

    public Dictionary<string, object?> Parameters { get; private set; }
    public StringBuilder Builder { get; private set; }

    private readonly char[] _forbiddenFieldNameCharacters;

    public Query(QueryBuilderOptions options)
        : this(options, new Dictionary<string, object?>(), new StringBuilder()) { }

    internal Query(QueryBuilderOptions options, Dictionary<string, object?> parameters, StringBuilder sb) {
        Options = options;
        Parameters = parameters;
        Builder = sb;

        _forbiddenFieldNameCharacters = options.SqlFlavor.ForbiddenFieldNameCharacters();
    }

    public override string ToString() => ToParameterizedSql();

    public string ToParameterizedSql() {
        if (Parameters.Count > 0) {
            Parameters = new Dictionary<string, object?>();
            Builder = new StringBuilder();
        }

        return ToParameterizedSqlInternal();
    }

    internal string ToParameterizedSqlInternal() {
        if (RawQueryParts.Count > 0) {
            AppendQueryParts(RawQueryParts);
        }

        AppendInsertParts();

        if (UpdateTable is not null) {
            Builder.Append("update ");
            UpdateTable.AppendToQuery(this, true);
            Builder.Append(" set ");
            AppendQueryParts(UpdateValues);
        }

        if (DeleteTable is not null) {
            Builder.Append("delete from ");
            DeleteTable.AppendToQuery(this, true);
        }

        AppendSelectFromParts();

        if (Joins.Count > 0) {
            AppendQueryParts(Joins);
        }

        if (WhereForest.Count > 0) {
            AppendQueryParts(WhereForest);
        }

        if (GroupByColumns.Count > 0) {
            Builder.Append(" group by ");
            AppendQueryParts(GroupByColumns);
        }

        if (HavingForest.Count > 0) {
            AppendQueryParts(HavingForest);
        }

        if (OrderByList.Count > 0) {
            Builder.Append(" order by ");
            AppendQueryParts(OrderByList);
        }

        if (Limits.Count > 0) {
            AppendQueryParts(Limits);
        }

        return Builder.ToString();
    }

    private void AppendInsertParts() {
        if (InsertTable is not null) {
            Builder.Append("insert into ");
            InsertTable.AppendToQuery(this, true);
            if (InsertColumns.Count > 0) {
                Builder.Append(" (");
                AppendQueryParts(InsertColumns);
                Builder.Append(')');
            }
            bool isFirst = true;
            foreach (var chunk in InsertValues.Chunk(InsertColumns.Count == 0 ? InsertValues.Count : InsertColumns.Count)) {
                if (chunk.Count() != InsertColumns.Count && InsertColumns.Count != 0) {
                    throw new InvalidOperationException("Wrong number of insert values.");
                }
                Builder.Append(isFirst ? " values (" : ", (");
                AppendQueryParts(chunk);
                Builder.Append(')');
                isFirst = false;
            }
        }
    }

    private void AppendSelectFromParts() {
        if (SelectColumns.Count > 0) {
            if (Builder.Length > 0) {
                Builder.Append(' ');
            }
            Builder.Append("select ");
            if (Distinct) {
                Builder.Append("distinct ");
            }
            AppendQueryParts(SelectColumns);
        }
        if (SelectFrom.Count > 0) {
            Builder.Append(" from ");
            AppendQueryParts(SelectFrom);
        }
    }

    public void AppendQueryParts(IEnumerable<IQueryPart> queryParts) {
        bool isFirst = true;
        foreach (var part in queryParts) {
            part.AppendToQuery(this, isFirst);
            isFirst = false;
        }
    }

    public void AppendParameter(object? parameter) {
        bool parameterize = true;
        string key = $"@{Parameters.Count}";

        if (parameter is null) {
            Builder.Append("null");
            parameterize = false;
        } else if (DateTimeParser.ParseQueryParameter(parameter, out string? parsedParameter)) {
            Builder.Append(key);
            parameter = parsedParameter;
        } else if (Options.DontParameterizeNumbers &&
                (parameter is int || parameter is long || parameter is byte || parameter is short
                || parameter is uint || parameter is ulong || parameter is sbyte || parameter is ushort)) {
            Builder.Append(parameter);
            parameterize = false;
        } else if (Options.DontParameterizeNumbers && parameter is bool booleanParameter) {
            Builder.Append(booleanParameter ? 1 : 0);
            parameterize = false;
        } else {
            Builder.Append(key);
        }

        if (parameterize) {
            Parameters.Add(key, parameter);
        }
    }

    public void AppendSubquery(QueryBuilder queryBuilder) {
        var (resultString, parameters) = queryBuilder.ToParameterizedSqlWithParams();
        Builder.Append(resultString);
        foreach (var p in parameters) {
            string key = $"@{Parameters.Count}";
            Parameters.Add(key, p.Value);
        }
    }

    public string WrapField(string fieldName) {
        if (Options.WrapFieldNames) {
            if (Options.OverprotectiveSqlInjection && _forbiddenFieldNameCharacters.Any(fieldName.Contains)) {
                throw new PotentialSqlInjectionException(string.Join(", ", _forbiddenFieldNameCharacters));
            }
            var wrappedNames = fieldName.Split('.').Select(n => {
                return n == "*" ? n : Options.SqlFlavor.WrapFieldName(n);
            });
            return string.Join('.', wrappedNames);
        }
        return fieldName;
    }

    public Query Clone() {
        var query = new Query(
            Options.Clone(),
            new Dictionary<string, object?>(Parameters),
            new StringBuilder(Builder.ToString())
        );

        query.RawQueryParts.AddRange(RawQueryParts);
        query.InsertTable = InsertTable;
        query.InsertColumns.AddRange(InsertColumns);
        query.InsertValues.AddRange(InsertValues);
        query.UpdateTable = UpdateTable;
        query.UpdateValues.AddRange(UpdateValues);
        query.DeleteTable = DeleteTable;
        query.SelectColumns.AddRange(SelectColumns);
        query.Distinct = Distinct;
        query.SelectFrom.AddRange(SelectFrom);
        query.Joins.AddRange(Joins);
        query.WhereForest.AddRange(WhereForest);
        query.GroupByColumns.AddRange(GroupByColumns);
        query.HavingForest.AddRange(HavingForest);
        query.OrderByList.AddRange(OrderByList);
        query.Limits.AddRange(Limits);

        return query;
    }
}
