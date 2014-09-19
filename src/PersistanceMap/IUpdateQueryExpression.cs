using System;
using System.Linq.Expressions;

namespace PersistanceMap
{
    public interface IUpdateQueryExpression
    {
        IUpdateQueryExpression AddToStore();

        /// <summary>
        /// Updates a row with the values provided by the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to update</typeparam>
        /// <param name="dataObject">Expression providing the object containing the data</param>
        /// <param name="where">The expression providing the where statement</param>
        /// <returns></returns>
        IUpdateQueryExpression Update<T>(Expression<Func<T>> dataObject, Expression<Func<T, object>> where = null);

        /// <summary>
        /// Updates a row with the values provided by the anonym dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to update</typeparam>
        /// <param name="anonym">Expression providing the anonym object containing the data</param>
        /// <param name="where">The expression providing the where statement</param>
        /// <returns></returns>
        IUpdateQueryExpression Update<T>(Expression<Func<object>> anonym, Expression<Func<T, bool>> where = null);
    }
}
