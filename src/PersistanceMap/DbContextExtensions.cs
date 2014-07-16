using PersistanceMap.Compiler;
using PersistanceMap.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder;

namespace PersistanceMap
{
    public static class DbContextExtensions
    {
        #region Select Expressions

        public static IEnumerable<T> Select<T>(this IDbContext context)
        {
            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(new SelectQueryPartsMap());

            return context.Execute<T>(query);
        }

        public static ISelectExpression<T> From<T>(this IDbContext context)
        {
            return new SelectExpression<T>(context)
                .From<T>();
        }

        public static ISelectExpression<T> From<T>(this IDbContext context, params Expression<Func<SelectMapOption<T>, IMapQueryPart>>[] parts)
        {
            return new SelectExpression<T>(context)
                .From<T>(MapOptionCompiler.Compile<T>(parts).ToArray());
        }

        public static ISelectExpression<T> From<T, TJoin>(this IDbContext context, Expression<Func<TJoin, T, bool>> predicate)
        {
            return new SelectExpression<T>(context)
                .From<T>()
                .Join<TJoin>(predicate);
        }

        #endregion

        #region Procedure Expressions

        public static IProcedureExpression Procedure(this IDbContext context, string procName)
        {
            return new ProcedureExpression(context, procName);
        }

        #endregion
    }
}
