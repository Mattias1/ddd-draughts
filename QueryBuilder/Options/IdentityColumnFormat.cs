namespace SqlQueryBuilder.Options;

public sealed class IdentityColumnFormat : IColumnFormat {
    public string Format(string entityColumn) => entityColumn;
}
