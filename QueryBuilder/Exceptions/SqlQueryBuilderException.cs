using System;

namespace SqlQueryBuilder.Exceptions;

public class SqlQueryBuilderException : Exception {
    public SqlQueryBuilderException(string message) : base(message) { }
    public SqlQueryBuilderException(string message, Exception? innerException) : base(message, innerException) { }
}
