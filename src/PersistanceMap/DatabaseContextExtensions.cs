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

        public static ISelectQueryExpression<T> From<T>(this IDatabaseContext context)
        {
            return new SelectQueryBuilder<T>(context)
                .From<T>();
        }

        public static ISelectQueryExpression<T> From<T>(this IDatabaseContext context, string alias)
        {
            return new SelectQueryBuilder<T>(context)
                .From<T>(alias);
        }

        public static ISelectQueryExpression<TJoin> From<T, TJoin>(this IDatabaseContext context, Expression<Func<TJoin, T, bool>> predicate)
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

        public static IDeleteQueryExpression Delete<T>(this IDatabaseContext context)
        {
            return new DeleteQueryBuilder(context)
                .Delete<T>()
                .AddToStore();
        }

        /// <summary>
        /// Deletes a record based on the where expression
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="context"></param>
        /// <param name="where">The expression defining the where statement</param>
        public static IDeleteQueryExpression Delete<T>(this IDatabaseContext context, Expression<Func<T, bool>> where)
        {
            return new DeleteQueryBuilder(context)
                .Delete(where)
                .AddToStore();
        }

        /// <summary>
        /// Deletes a record based on the Properties and values of the given entity
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="context"></param>
        /// <param name="dataObject">The entity to delete</param>
        /// <param name="where">The property defining the key on the entity</param>
        public static IDeleteQueryExpression Delete<T>(this IDatabaseContext context, Expression<Func<T>> dataObject, Expression<Func<T, object>> where = null)
        {
            return new DeleteQueryBuilder(context)
                .Delete(dataObject, where)
                .AddToStore();
        }

        /// <summary>
        /// Delete a record based on the Properties and values passed in the anonym object
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="context"></param>
        /// <param name="anonym">The object that defines the properties and the values that mark the object to delete</param>
        /// <returns>IDeleteQueryProvider</returns>
        public static IDeleteQueryExpression Delete<T>(this IDatabaseContext context, Expression<Func<object>> anonym)
        {
            return new DeleteQueryBuilder(context)
                .Delete<T>(anonym)
                .AddToStore();
        }

        #endregion

        #region Update Expressions

        public static IUpdateQueryExpression Update<T>(this IDatabaseContext context, Expression<Func<T>> dataObject, Expression<Func<T, object>> where = null)
        {
            // update all except the key elements used in the reference expression
            return new UpdateQueryBuilder(context)
                .Update(dataObject, where)
                .AddToStore();
        }

        public static IUpdateQueryExpression Update<T>(this IDatabaseContext context, Expression<Func<object>> anonym, Expression<Func<T, bool>> where = null)
        {
            // update all fields defined in the anonym object
            return new UpdateQueryBuilder(context)
                .Update(anonym, where)
                .AddToStore();
        }


        #endregion

        #region Insert Expressions

        public static IInsertQueryExpression Insert<T>(this IDatabaseContext context, Expression<Func<T>> dataObject)
        {
            throw new NotImplementedException();
        }

        public static IInsertQueryExpression Insert<T>(this IDatabaseContext context, Expression<Func<object>> anonym)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Procedure Expressions

        public static IProcedureQueryExpression Procedure(this IDatabaseContext context, string procName)
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
