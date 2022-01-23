using SqlQueryBuilder.Options;
using System.Linq;

namespace SqlQueryBuilder.Common;

public static class ISqlFlavorExtensions {
    public static char[] ForbiddenFieldNameCharacters(this ISqlFlavor sqlFlavor) {
        return sqlFlavor.WrapFieldName("").ToCharArray().Distinct().ToArray();
    }
}
