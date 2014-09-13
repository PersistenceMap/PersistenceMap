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
            new DeleteQueryBuilder(context)
                .Delete<T>()
                .AddToStore();
        }

        /// <summary>
        /// Deletes a record based on the where expression
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="context"></param>
        /// <param name="where">The expression defining the where statement</param>
        public static void Delete<T>(this IDatabaseContext context, Expression<Func<T, bool>> where)
        {
            new DeleteQueryBuilder(context)
                .Delete(where)
                .AddToStore();
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
            new DeleteQueryBuilder(context)
                .Delete(entity, key)
                .AddToStore();
        }

        public static void Delete<T>(this IDatabaseContext context, Expression<Func<object>> anonym)
        {
            new DeleteQueryBuilder(context)
                .Delete<T>(anonym)
                .AddToStore();
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

        #region Map

        /// <summary>
        /// Maps the output from the reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this IDatabaseContext context, IReaderContext reader)
        {
            return context.Kernel.Map<T>(reader);
        }

        /// <summary>
        /// Maps the output from the reader to the provided fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this IDatabaseContext context, IReaderContext reader, FieldDefinition[] fields)
        {
            return context.Kernel.Map<T>(reader, fields);
        }

        #endregion
    }
}
