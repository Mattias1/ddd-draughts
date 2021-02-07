using System;

namespace SqlQueryBuilder.Exceptions {
    public class SqlExecutionException : SqlQueryBuilderException {
        public string? ParameterizedSql { get; }

        public SqlExecutionException(Exception innerException, string? parameterizedSql)
                : base(innerException.Message, innerException) {
            ParameterizedSql = parameterizedSql;
        }
    }
}
