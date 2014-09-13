using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public static class DatabaseContextExtensions
    {
        #region Select Expressions

        public static IEnumerable<T> Select<T>(this IDatabaseContext context)
        {
            var builder = new SelectQueryBuilder<T>(context)
                .From<T>();

            return builder.Select<T>();
        }

        public static ISelectQueryProvider<T> From<T>(this IDatabaseContext context)
        {
            return new SelectQueryBuilder<T>(context)
                .From<T>();
        }

        public static ISelectQueryProvider<T> From<T>(this IDatabaseContext context, string alias)
        {
            return new SelectQueryBuilder<T>(context)
                .From<T>(alias);
        }

        public static ISelectQueryProvider<TJoin> From<T, TJoin>(this IDatabaseContext context, Expression<Func<TJoin, T, bool>> predicate)
        {
            return new SelectQueryBuilder<T>(context)
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
            var builder = new DeleteQueryBuilder(context);
            builder.Delete<T>();
        }

        /// <summary>
        /// Deletes a record based on the where expression
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="context"></param>
        /// <param name="where">The expression defining the where statement</param>
        public static void Delete<T>(this IDatabaseContext context, Expression<Func<T, bool>> where)
        {
            var builder = new DeleteQueryBuilder(context);
            builder.Delete(where);
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
            var builder = new DeleteQueryBuilder(context);
            builder.Delete(entity, key);
        }

        public static void Delete<T>(this IDatabaseContext context, Expression<Func<object>> anonym)
        {
            var builder = new DeleteQueryBuilder(context);
            builder.Delete<T>(anonym);
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
