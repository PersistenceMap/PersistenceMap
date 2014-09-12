using PersistanceMap.Compiler;
using PersistanceMap.Internals;
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
            var queryParts = new SelectQueryPartsMap();
            
            QueryPartsFactory.AppendSimpleQueryPart(queryParts, OperationType.Select);

            QueryPartsFactory.AppendEntityQueryPart<T>(queryParts, OperationType.From);

            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(queryParts);

            return context.Execute<T>(query);
        }

        public static ISelectQueryProvider<T> From<T>(this IDatabaseContext context)
        {
            return new SelectQueryProvider<T>(context)
                .From<T>();
        }

        public static ISelectQueryProvider<T> From<T>(this IDatabaseContext context, string alias)
        {
            return new SelectQueryProvider<T>(context)
                .From<T>(alias);
        }

        public static ISelectQueryProvider<TJoin> From<T, TJoin>(this IDatabaseContext context, Expression<Func<TJoin, T, bool>> predicate)
        {
            return new SelectQueryProvider<T>(context)
                .From<T>()
                .Join<TJoin>(predicate);
        }

        public static IEnumerable<T> Select<T>(this IDatabaseContext context, string queryString)
        {
            var query = new CompiledQuery
            {
                QueryString = queryString
            };

            return context.Execute<T>(query);
        }

        #endregion

        #region Delete Expressions

        public static void Delete<T>(this IDatabaseContext context)
        {
            var queryParts = new QueryPartsMap();

            QueryPartsFactory.AppendSimpleQueryPart(queryParts, OperationType.Delete);

            QueryPartsFactory.AppendEntityQueryPart<T>(queryParts, OperationType.From);

            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(queryParts);

            context.Execute(query);
        }

        /// <summary>
        /// Deletes a record based on the where expression
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="context"></param>
        /// <param name="where">The expression defining the where statement</param>
        public static void Delete<T>(this IDatabaseContext context, Expression<Func<T, bool>> where)
        {
            var queryParts = new QueryPartsMap();

            QueryPartsFactory.AppendSimpleQueryPart(queryParts, OperationType.Delete);

            QueryPartsFactory.AppendEntityQueryPart<T>(queryParts, OperationType.From);

            QueryPartsFactory.AppendExpressionQueryPart(queryParts, where, OperationType.Where);

            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(queryParts);

            context.Execute(query);
        }

        /// <summary>
        /// Deletes a record based on the Properties and values of the given entity
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="context"></param>
        /// <param name="entity">The entity to delete</param>
        /// <param name="key">The property defining the key on the entity</param>
        public static void Delete<T>(this IDatabaseContext context, Expression<Func<T>> entity, Expression<Func<T, object>> key = null)
        {
            // create expression containing key and value
            var whereexpr = ExpressionFactory.CreateKeyExpression(entity, key);
            if (whereexpr == null)
            {
                // find the property called ID or {objectname}ID to define the where expression
                whereexpr = ExpressionFactory.CreateKeyExpression(entity);
            }

            var queryParts = new QueryPartsMap();

            QueryPartsFactory.AppendSimpleQueryPart(queryParts, OperationType.Delete);

            QueryPartsFactory.AppendEntityQueryPart<T>(queryParts, OperationType.From);

            QueryPartsFactory.AppendExpressionQueryPart(queryParts, whereexpr, OperationType.Where);

            var expr = context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(queryParts);

            context.Execute(query);
        }

        public static void Delete<T>(this IDatabaseContext context, Expression<Func<object>> anonym)
        {
            // delete item that matches all properties set in the object
            throw new NotImplementedException();
        }

        #endregion

        #region Update Expressions

        public static void Update<T>(this IDatabaseContext context, Expression<Func<T>> entity, Expression<Func<T, object>> key = null)
        {
            // update all except the key elements used in the reference expression
            if (key == null)
            {
                var keyexpr = ExpressionFactory.CreateKeyExpression(entity);
            }

            throw new NotImplementedException();
        }

        public static void Update<T>(this IDatabaseContext context, Expression<Func<object>> anonym, Expression<Func<T, bool>> predicate)
        {
            // update all fields defined in the anonym object
            throw new NotImplementedException();
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
