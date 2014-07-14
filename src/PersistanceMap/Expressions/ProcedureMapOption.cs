using PersistanceMap.QueryBuilder;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.Expressions
{
    /// <summary>
    /// MapOption for procedures
    /// </summary>
    public class ProcedureMapOption
    {
        public IExpressionMapQueryPart Name(Expression<Func<string>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Identifier, predicate);
        }

        public IExpressionMapQueryPart Value<T>(Expression<Func<T>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Value, predicate);
        }
    }
}
