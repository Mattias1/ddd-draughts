namespace SqlQueryBuilder.Model {
    internal readonly struct WhereLeaf : IWhere {
        public enum ValueType { Any, Column }

        public Where.WhereType WhereType { get; }
        public string ColumnName { get; }
        public Comparison[] Comparisons { get; }

        public WhereLeaf(Where.WhereType whereType, string name, string @operator, object? value, ValueType type = ValueType.Any)
            : this(whereType, name, new[] { new Comparison(@operator, value, type) }) { }

        public WhereLeaf(Where.WhereType whereType, string name, params Comparison[] comparisons) {
            WhereType = whereType;
            ColumnName = name;
            Comparisons = comparisons;
        }

        public void AppendToQuery(Query query, bool isFirst) {
            query.Builder.Append(' ').Append(Where.WhereTypeToString(WhereType, isFirst))
                .Append(' ').Append(ColumnName);

            foreach (var comparison in Comparisons) {
                query.Builder.Append(' ').Append(comparison.OperatorName).Append(' ');
                if (comparison.Type == ValueType.Column) {
                    query.Builder.Append(comparison.Value);
                }
                else {
                    query.AppendParameter(comparison.Value);
                }
            }
        }

        internal readonly struct Comparison {
            public string OperatorName { get; }
            public object? Value { get; }
            public ValueType Type { get; }

            public Comparison(string name, object? value, ValueType type = ValueType.Any) {
                OperatorName = name;
                Value = value;
                Type = type;
            }
        }
    }
}
