using System;
using System.Collections.Generic;
using System.Text;

namespace SqlQueryBuilder.Model;

internal interface IWhere : IQueryPart { }

internal readonly struct Where : IWhere {
    public enum WhereType { And, Or, AndNot, OrNot, AndHaving, OrHaving, AndNotHaving, OrNotHaving };

    public WhereType Type { get; }
    public List<IWhere> WhereForest { get; }

    public Where(WhereType type, List<IWhere> whereForest) {
        Type = type;
        WhereForest = whereForest;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        var recursiveQuery = new Query(query.Options, query.Parameters, new StringBuilder());
        recursiveQuery.AppendQueryParts(WhereForest);

        string resultString = recursiveQuery.ToParameterizedSqlInternal();
        if (resultString.StartsWith(" where ")) {
            resultString = resultString.Substring(7);
        } else if (resultString.StartsWith(" having ")) {
            resultString = resultString.Substring(8);
        }

        query.Builder.Append(' ').Append(WhereTypeToString(Type, isFirst)).Append(" (").Append(resultString).Append(')');
    }

    internal static string WhereTypeToString(WhereType type, bool isFirst) {
        if (isFirst && (type == WhereType.Or || type == WhereType.OrNot
                || type == WhereType.OrHaving || type == WhereType.OrNotHaving)) {
            throw new InvalidOperationException("No 'Where' (or 'And') is added just yet, why are you adding an 'Or'?");
        }

        return type switch {
            WhereType.And => isFirst ? "where" : "and",
            WhereType.Or => "or",
            WhereType.AndNot => isFirst ? "where not" : "and not",
            WhereType.OrNot => "or not",

            WhereType.AndHaving => isFirst ? "having" : "and",
            WhereType.OrHaving => "or",
            WhereType.AndNotHaving => isFirst ? "having not" : "and not",
            WhereType.OrNotHaving => "or not",

            _ => throw new NotImplementedException(),
        };
    }
}
