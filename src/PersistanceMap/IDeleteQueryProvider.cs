using System;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IDeleteQueryProvider : IQueryProvider
    {
        IDeleteQueryProvider AddToStore();

        IDeleteQueryProvider Delete<T>();

        /// <summary>
        /// Deletes a record based on the where expression
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="where">The expression defining the where statement</param>
        /// <returns>IDeleteQueryProvider</returns>
        IDeleteQueryProvider Delete<T>(Expression<Func<T, bool>> where);

        /// <summary>
        /// Deletes a record based on the Properties and values of the given entity
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="dataObject">The entity to delete</param>
        /// <param name="where">The property defining the key on the entity</param>
        /// <returns>IDeleteQueryProvider</returns>
        IDeleteQueryProvider Delete<T>(Expression<Func<T>> dataObject, Expression<Func<T, object>> where = null);

        /// <summary>
        /// Delete a record based on the Properties and values passed in the anonym object
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="anonym">The object that defines the properties and the values that mark the object to delete</param>
        /// <returns>IDeleteQueryProvider</returns>
        IDeleteQueryProvider Delete<T>(Expression<Func<object>> anonym);
    }
}
