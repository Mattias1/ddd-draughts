using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Builder;

public interface IQueryBuilderBase {
    ICompleteQueryBuilder Cast();

    bool Execute();
    Task<bool> ExecuteAsync();

    Task<IReadOnlyList<T>> ListAsync<T>();
    IReadOnlyList<T> List<T>();

    string ToString();
    string ToUnsafeSql();
    string ToParameterizedSql();

    ICompleteQueryBuilder Clone();
    ICompleteQueryBuilder CloneWithoutSelect();
}
