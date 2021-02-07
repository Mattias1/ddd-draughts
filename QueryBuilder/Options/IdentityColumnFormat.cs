namespace SqlQueryBuilder.Options {
    public class IdentityColumnFormat : IColumnFormat {
        public string Format(string entityColumn) => entityColumn;
    }
}
