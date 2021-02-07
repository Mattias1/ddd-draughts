namespace SqlQueryBuilder.Exceptions {
    public class PotentialSqlInjectionException : SqlQueryBuilderException {
        public const string ERROR_CHARACTERS = "Potentially dangerous sql detected, is someone trying SQL-injection?";

        public PotentialSqlInjectionException(string message) : base(message) { }

        public static PotentialSqlInjectionException ForDangerousCharacters() => new PotentialSqlInjectionException(ERROR_CHARACTERS);
    }
}
