namespace SqlQueryBuilder.Model {
    internal interface ISetColumn : IQueryPart { }

    internal readonly struct SetColumn : ISetColumn {
        public enum ValueType { Any, Column }

        public string ColumnName { get; }
        public object? Value { get; }

        public SetColumn(string name, object? value) {
            ColumnName = name;
            Value = value;
        }

        public void AppendToQuery(Query query, bool isFirst) {
            if (!isFirst) {
                query.Builder.Append(", ");
            }
            query.Builder.Append(ColumnName).Append(" = ");
            query.AppendParameter(Value);
        }
    }
}
