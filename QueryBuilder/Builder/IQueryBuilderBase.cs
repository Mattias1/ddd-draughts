using SqlQueryBuilder.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Builder;

public interface IQueryBuilderBase {
    ICompleteQueryBuilder Cast();

    SqlBuilderResultRow FirstResult();
    SqlBuilderResultRow? FirstOrDefaultResult();
    SqlBuilderResultRow SingleResult();
    SqlBuilderResultRow? SingleOrDefaultResult();
    IReadOnlyList<SqlBuilderResultRow> Results();

    Task<SqlBuilderResultRow> FirstResultAsync();
    Task<SqlBuilderResultRow?> FirstOrDefaultResultAsync();
    Task<SqlBuilderResultRow> SingleResultAsync();
    Task<SqlBuilderResultRow?> SingleOrDefaultResultAsync();
    Task<IReadOnlyList<SqlBuilderResultRow>> ResultsAsync();

    bool Execute();
    Task<bool> ExecuteAsync();

    string ToString();
    string ToUnsafeSql();
    string ToParameterizedSql();

    ICompleteQueryBuilder Clone();
    ICompleteQueryBuilder CloneWithoutSelect();
}
