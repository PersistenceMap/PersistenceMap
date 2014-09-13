using System;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IDeleteQueryProvider
    {
        void AddToStore();

        IDeleteQueryProvider Delete<T>();

        /// <summary>
        /// Deletes a record based on the where expression
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="where">The expression defining the where statement</param>
        IDeleteQueryProvider Delete<T>(Expression<Func<T, bool>> where);

        /// <summary>
        /// Deletes a record based on the Properties and values of the given entity
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="entity">The entity to delete</param>
        /// <param name="key">The property defining the key on the entity</param>
        IDeleteQueryProvider Delete<T>(Expression<Func<T>> entity, Expression<Func<T, object>> key = null);

        IDeleteQueryProvider Delete<T>(Expression<Func<object>> anonym);
    }
}
