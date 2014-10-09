using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public static class DatabaseContextExtensions
    {
        #region Execute

        public static IEnumerable<T> Execute<T>(this IDatabaseContext context, string queryString)
        {
            var query = new CompiledQuery
            {
                QueryString = queryString
            };

            return context.Kernel.Execute<T>(query);
        }

        public static void Execute(this IDatabaseContext context, string queryString)
        {
            var query = new CompiledQuery
            {
                QueryString = queryString
            };

            context.Kernel.Execute(query);
        }

        #endregion

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

        /// <summary>
        /// Updates a row with the values provided by the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to update</typeparam>
        /// <param name="context"></param>
        /// <param name="dataObject">Expression providing the object containing the data</param>
        /// <param name="where">The expression providing the where statement</param>
        /// <returns></returns>
        public static IUpdateQueryExpression<T> Update<T>(this IDatabaseContext context, Expression<Func<T>> dataObject, Expression<Func<T, object>> where = null)
        {
            return new UpdateQueryBuilder<T>(context)
                .Update(dataObject, where)
                .AddToStore();
        }

        /// <summary>
        /// Updates a row with the values provided by the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to update</typeparam>
        /// <param name="context"></param>
        /// <param name="anonym">Expression providing the anonym object containing the data1</param>
        /// <param name="where">The expression providing the where statement</param>
        /// <returns></returns>
        public static IUpdateQueryExpression<T> Update<T>(this IDatabaseContext context, Expression<Func<object>> anonym, Expression<Func<T, bool>> where = null)
        {
            return new UpdateQueryBuilder<T>(context)
                .Update(anonym, where)
                .AddToStore();
        }


        #endregion

        #region Insert Expressions

        /// <summary>
        /// Inserts a row with the values defined in the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to insert</typeparam>
        /// <param name="context"></param>
        /// <param name="dataObject">Expression providing the object containing the data</param>
        /// <returns></returns>
        public static IInsertQueryExpression<T> Insert<T>(this IDatabaseContext context, Expression<Func<T>> dataObject)
        {
            return new InsertQueryBuilder<T>(context)
                .Insert(dataObject)
                .AddToStore();
        }

        /// <summary>
        /// Inserts a row with the values defined in the anonym dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to insert</typeparam>
        /// <param name="context"></param>
        /// <param name="anonym">Expression providing the anonym object containing the data</param>
        /// <returns></returns>
        public static IInsertQueryExpression<T> Insert<T>(this IDatabaseContext context, Expression<Func<object>> anonym)
        {
            return new InsertQueryBuilder<T>(context)
                .Insert(anonym)
                .AddToStore();
        }

        #endregion

        #region Procedure Expressions

        public static IProcedureQueryExpression Procedure(this IDatabaseContext context, string procName)
        {
            return new ProcedureQueryProvider(context, procName);
        }

        #endregion

        //#region Map

        ///// <summary>
        ///// Maps the output from the reader
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="reader"></param>
        ///// <returns></returns>
        //public static IEnumerable<T> Map<T>(this IDatabaseContext context, IReaderContext reader)
        //{
        //    return context.Kernel.Map<T>(reader);
        //}

        ///// <summary>
        ///// Maps the output from the reader to the provided fields
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="reader"></param>
        ///// <param name="fields"></param>
        ///// <returns></returns>
        //public static IEnumerable<T> Map<T>(this IDatabaseContext context, IReaderContext reader, FieldDefinition[] fields)
        //{
        //    return context.Kernel.Map<T>(reader, fields);
        //}

        //#endregion
    }
}
