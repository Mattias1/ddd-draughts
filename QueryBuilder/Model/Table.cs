namespace SqlQueryBuilder.Model {
    internal readonly struct Table {
        public string TableName { get; }
        public string? Alias { get; }

        public Table(string name, string? alias = null) {
            TableName = name;
            Alias = alias;
        }

        public override string ToString() => Alias is null ? TableName : $"{TableName} as {Alias}";
    }
}
