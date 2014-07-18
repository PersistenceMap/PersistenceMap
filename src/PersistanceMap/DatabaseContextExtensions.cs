using PersistanceMap.Compiler;
using PersistanceMap.QueryProvider;
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

        public static ISelectQueryProvider<T> From<T>(this IDatabaseContext context)
        {
            return new SelectQueryProvider<T>(context)
                .From<T>();
        }

        public static ISelectQueryProvider<T> From<T>(this IDatabaseContext context, params Expression<Func<SelectMapOption<T>, IQueryMap>>[] parts)
        {
            return new SelectQueryProvider<T>(context)
                .From<T>(MapOptionCompiler.Compile<T>(parts).ToArray());
        }

        public static ISelectQueryProvider<T> From<T, TJoin>(this IDatabaseContext context, Expression<Func<TJoin, T, bool>> predicate)
        {
            return new SelectQueryProvider<T>(context)
                .From<T>()
                .Join<TJoin>(predicate);
        }

        #endregion

        #region Procedure Expressions

        public static IProcedureQueryProvider Procedure(this IDatabaseContext context, string procName)
        {
            return new ProcedureQueryProvider(context, procName);
        }

        #endregion
    }
}
