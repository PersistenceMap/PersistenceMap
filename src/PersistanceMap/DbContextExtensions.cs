using PersistanceMap.Expressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public static class DbContextExtensions
    {
        public static IEnumerable<T> Select<T>(this IDbContext context)
        {
            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(new QueryPartsContainer());

            return context.Execute<T>(query);
        }

        public static ISelectExpression<T> From<T>(this IDbContext context)
        {
            return new SelectExpression<T>(context)
                .From<T>();
        }

        public static ISelectExpression<T> From<T>(this IDbContext context, params Expression<Func<MapOption<T>, IExpressionMapQueryPart>>[] parts)
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
    }
}
