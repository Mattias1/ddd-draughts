namespace SqlQueryBuilder.Exceptions {
    public class PotentialSqlInjectionException : SqlQueryBuilderException {
        public const string ERROR_CHARACTERS = "Potentially dangerous sql detected, is someone trying SQL-injection?";

        public PotentialSqlInjectionException() : this(ERROR_CHARACTERS) { }
        public PotentialSqlInjectionException(string message) : base(message) { }
    }
}
