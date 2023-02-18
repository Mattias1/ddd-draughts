namespace SqlQueryBuilder.Model;

internal readonly struct WhereLeaf : IWhere {
    public enum ValueType { Any, Column }

    public Where.WhereType WhereType { get; }
    public string ColumnName { get; }
    public IComparison[] Comparisons { get; }

    public WhereLeaf(Where.WhereType whereType, string name, string @operator, object? value, ValueType type = ValueType.Any)
        : this(whereType, name, new Comparison(@operator, value, type)) { }

    public WhereLeaf(Where.WhereType whereType, string name, params IComparison[] comparisons) {
        WhereType = whereType;
        ColumnName = name;
        Comparisons = comparisons;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        query.Builder
            .Append(' ').Append(Where.WhereTypeToString(WhereType, isFirst))
            .Append(' ').Append(query.WrapField(ColumnName));

        query.AppendQueryParts(Comparisons);
    }

    internal interface IComparison : IQueryPart { }

    internal readonly struct Comparison : IComparison {
        public string OperatorName { get; }
        public object? Value { get; }
        public ValueType Type { get; }

        public Comparison(string name, object? value, ValueType type = ValueType.Any) {
            OperatorName = name;
            Value = value;
            Type = type;
        }

        public void AppendToQuery(Query query, bool isFirst) {
            query.Builder.Append(' ').Append(OperatorName).Append(' ');
            if (Type == ValueType.Column && Value is string columnValue) {
                query.Builder.Append(query.WrapField(columnValue));
            } else {
                query.AppendParameter(Value);
            }
        }
    }
}
