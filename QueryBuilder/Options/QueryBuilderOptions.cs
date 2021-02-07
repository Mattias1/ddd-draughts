namespace SqlQueryBuilder.Options {
    public class QueryBuilderOptions {
        public ISqlFlavor SqlFlavor { get; set; }
        public IColumnFormat ColumnFormat { get; set; }
        public bool SmartDate { get; set; }
        public bool OverprotectiveSqlInjection { get; set; }
        public bool AddParameterizedSqlToException { get; set; }
        public bool DontParameterizeNumbers { get; set; }

        public QueryBuilderOptions(ISqlFlavor sqlFlavor) : this(sqlFlavor, Default) { }
        public QueryBuilderOptions(ISqlFlavor sqlFlavor, IColumnFormat columnFormat)
            : this(sqlFlavor, columnFormat, smartDate: true, overprotective: true, addSqlToException: true,
                  dontParameterizeNumbers: true) { }
        public QueryBuilderOptions(ISqlFlavor sqlFlavor, IColumnFormat columnFormat,
                bool smartDate, bool overprotective, bool addSqlToException, bool dontParameterizeNumbers) {
            SqlFlavor = sqlFlavor;
            ColumnFormat = columnFormat;
            SmartDate = smartDate;
            OverprotectiveSqlInjection = overprotective;
            AddParameterizedSqlToException = addSqlToException;
            DontParameterizeNumbers = dontParameterizeNumbers;
        }

        public static QueryBuilderOptions SmartPreset(ISqlFlavor sqlFlavor) => SmartPreset(sqlFlavor, Default);
        public static QueryBuilderOptions SmartPreset(ISqlFlavor sqlFlavor, IColumnFormat columnFormat) {
            return new QueryBuilderOptions(sqlFlavor, columnFormat,
                smartDate: true, overprotective: true, addSqlToException: true, dontParameterizeNumbers: true);
        }

        public static QueryBuilderOptions PlainPreset(ISqlFlavor sqlFlavor) => PlainPreset(sqlFlavor, None);
        public static QueryBuilderOptions PlainPreset(ISqlFlavor sqlFlavor, IColumnFormat columnFormat) {
            return new QueryBuilderOptions(sqlFlavor, columnFormat,
                smartDate: false, overprotective: false, addSqlToException: false, dontParameterizeNumbers: false);
        }

        public static IColumnFormat Default => CamelToSnakeCase;
        public static IColumnFormat None => new IdentityColumnFormat();
        public static IColumnFormat CamelToSnakeCase => new CamelToSnakeColumnFormat();
    }
}
