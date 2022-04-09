namespace SqlQueryBuilder.Exceptions;

public sealed class PotentialSqlInjectionException : SqlQueryBuilderException {
    public const string ERROR_CHARACTERS = "Potentially dangerous sql detected, is someone trying SQL-injection?";

    public PotentialSqlInjectionException() : this("") { }
    public PotentialSqlInjectionException(string characters) : base(CharacterErrorMessage(characters)) { }

    private static string CharacterErrorMessage(string characters) {
        if (string.IsNullOrEmpty(characters)) {
            return ERROR_CHARACTERS;
        }
        return $"{ERROR_CHARACTERS} Characters: {characters}.";
    }
}
