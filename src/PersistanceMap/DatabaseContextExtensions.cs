using PersistanceMap.Compiler;
using PersistanceMap.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public static class DatabaseContextExtensions
    {
        #region Select Expressions

        public static IEnumerable<T> Select<T>(this IDatabaseContext context)
        {
            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(new SelectQueryPartsMap());

            return context.Execute<T>(query);
        }

        public static ISelectExpression<T> From<T>(this IDatabaseContext context)
        {
            return new SelectExpression<T>(context)
                .From<T>();
        }

        public static ISelectExpression<T> From<T>(this IDatabaseContext context, params Expression<Func<SelectMapOption<T>, IMapQueryPart>>[] parts)
        {
            return new SelectExpression<T>(context)
                .From<T>(MapOptionCompiler.Compile<T>(parts).ToArray());
        }

        public static ISelectExpression<T> From<T, TJoin>(this IDatabaseContext context, Expression<Func<TJoin, T, bool>> predicate)
        {
            return new SelectExpression<T>(context)
                .From<T>()
                .Join<TJoin>(predicate);
        }

        #endregion

        #region Procedure Expressions

        public static IProcedureExpression Procedure(this IDatabaseContext context, string procName)
        {
            return new ProcedureExpression(context, procName);
        }

        #endregion
    }
}
