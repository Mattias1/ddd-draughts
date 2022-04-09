using System.Linq;
using System.Text.RegularExpressions;

namespace SqlQueryBuilder.Options;

public sealed class CamelToSnakeColumnFormat : IColumnFormat {
    private static readonly Regex UpperCamelCaseRegex = new Regex(@"^[^A-Z]+|[A-Z][^A-Z]*", RegexOptions.Compiled);

    public string Format(string entityColumn) {
        var matches = UpperCamelCaseRegex.Matches(entityColumn);
        return string.Join('_', matches.Select(m => m.Value.ToLower()));
    }
}
