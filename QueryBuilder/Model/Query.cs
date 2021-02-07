using SqlQueryBuilder.Common;
using SqlQueryBuilder.Options;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;

namespace SqlQueryBuilder.Model {
    internal class Query {
        public QueryBuilderOptions Options { get; }

        public List<IQueryPart> RawQueryParts { get; } = new List<IQueryPart>();

        public Table? InsertTable { get; set; }
        public List<IColumn> InsertColumns { get; } = new List<IColumn>();
        public List<IInsertValue> InsertValues { get; } = new List<IInsertValue>();

        public Table? UpdateTable { get; set; }
        public List<ISetColumn> UpdateValues { get; } = new List<ISetColumn>();

        public Table? DeleteTable { get; set; }

        public List<IColumn> SelectColumns { get; } = new List<IColumn>();
        public bool Distinct { get; set; } = false;

        public List<Table> SelectFrom { get; } = new List<Table>();
        public List<IJoin> Joins { get; } = new List<IJoin>();
        public List<IWhere> WhereForest { get; } = new List<IWhere>();
        public List<IColumn> GroupByColumns { get; } = new List<IColumn>();
        public List<IWhere> HavingForest { get; } = new List<IWhere>();
        public List<IOrderBy> OrderByList { get; } = new List<IOrderBy>();

        public Dictionary<string, object?> Parameters { get; private set; }
        public StringBuilder Builder { get; private set; }

        public Query(QueryBuilderOptions options) : this(options, new Dictionary<string, object?>(), new StringBuilder()) { }

        internal Query(QueryBuilderOptions options, Dictionary<string, object?> parameters, StringBuilder sb) {
            Options = options;
            Parameters = parameters;
            Builder = sb;
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

            if (InsertTable != null) {
                Builder.Append("insert into ").Append(InsertTable);
                if (InsertColumns.Count > 0) {
                    Builder.Append(" (");
                    AppendQueryParts(InsertColumns);
                    Builder.Append(')');
                }
                bool isFirst = true;
                foreach (var chunk in InsertValues.Chunk(InsertColumns.Count)) {
                    if (chunk.Count() != InsertColumns.Count && InsertColumns.Count != 0) {
                        throw new InvalidOperationException("Wrong number of insert values.");
                    }
                    Builder.Append(isFirst ? " values (" : ", (");
                    AppendQueryParts(chunk);
                    Builder.Append(')');
                    isFirst = false;
                }
            }

            if (UpdateTable != null) {
                Builder.Append("update ").Append(UpdateTable).Append(" set ");
                AppendQueryParts(UpdateValues);
            }

            if (DeleteTable != null) {
                Builder.Append("delete from ").Append(DeleteTable);
            }

            if (SelectColumns.Count > 0 || SelectFrom.Count > 0) {
                Builder.Append("select ");
                if (Distinct) {
                    Builder.Append("distinct ");
                }
                AppendQueryParts(SelectColumns);
                Builder.Append(" from ").AppendJoin(", ", SelectFrom);
            }

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

            return Builder.ToString();
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
            }
            else if (DateTimeParser.ParseQueryParameter(parameter, out string? parsedParameter)) {
                Builder.Append(key);
                parameter = parsedParameter;
            }
            else if (Options.DontParameterizeNumbers &&
                    (parameter is int || parameter is long || parameter is byte || parameter is short
                    || parameter is uint || parameter is ulong || parameter is sbyte || parameter is ushort)) {
                Builder.Append(parameter);
                parameterize = false;
            }
            else {
                Builder.Append(key);
            }

            if (parameterize) {
                Parameters.Add(key, parameter);
            }
        }
    }
}
