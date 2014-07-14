using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.Expressions
{
    /// <summary>
    /// MapOption for simple select expressions like From{T}
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectMapOption<T>
    {
        /// <summary>
        /// Creates a include expression to mark fields of the curent join that have to be included in the select statement
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart Include<T2>(Expression<Func<T, T2>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Include, predicate);
        }

        /// <summary>
        /// Adds an identifier to the fields of the curent join
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart Identifier(Expression<Func<string>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Identifier, predicate);
        }
    }

    /// <summary>
    /// MapOption for extended select expressions like Join{T,T2}
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class SelectMapOption<T, T2> : SelectMapOption<T>
    {
        #region On

        /// <summary>
        /// Provides an expression to mark the fields that have to be joined together
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart On(Expression<Func<T, T2, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Join, predicate);
        }

        /// <summary>
        /// Provides an expression to mark the fields that have to be joined together
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart On(string identifier, Expression<Func<T, T2, bool>> predicate)
        {
            var part = new ExpressionMapQueryPart(MapOperationType.Join, predicate);
            part.AddIdentifier(typeof(T2), identifier);

            return part;
        }

        /// <summary>
        /// Provides an expression to mark the fields that have to be joined together
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart On<T3>(Expression<Func<T, T3, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Join, predicate);
        }

        /// <summary>
        /// Provides an expression to mark the fields that have to be joined together
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart On<T3>(string identifier, Expression<Func<T, T3, bool>> predicate)
        {
            var part = new ExpressionMapQueryPart(MapOperationType.Join, predicate);
            part.AddIdentifier(typeof(T3), identifier);

            return part;
        }

        #endregion

        #region And

        /// <summary>
        /// Provides an expression to mark the fields that have to be joined together with a and expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart And(Expression<Func<T, T2, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.And, predicate);
        }

        /// <summary>
        /// Provides an expression to mark the fields that have to be joined together with a and expression
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart And(string identifier, Expression<Func<T, T2, bool>> predicate)
        {
            var part = new ExpressionMapQueryPart(MapOperationType.And, predicate);
            part.AddIdentifier(typeof(T2), identifier);

            return part;
        }

        /// <summary>
        /// Provides an expression to mark the fields that have to be joined together with a and expression
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart And<T3>(Expression<Func<T, T3, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.And, predicate);
        }

        /// <summary>
        /// Provides an expression to mark the fields that have to be joined together with a and expression
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart And<T3>(string identifier, Expression<Func<T, T3, bool>> predicate)
        {
            var part = new ExpressionMapQueryPart(MapOperationType.And, predicate);
            part.AddIdentifier(typeof(T3), identifier);

            return part;
        }

        #endregion

        #region Or

        /// <summary>
        /// Provides an expression to mark the fields that have to be joined together with a or expression
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart Or(Expression<Func<T, T2, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Or, predicate);
        }

        /// <summary>
        /// Provides an expression to mark the fields that have to be joined together with a or expression
        /// </summary>
        /// <typeparam name="T3"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IExpressionMapQueryPart Or<T3>(Expression<Func<T, T3, bool>> predicate)
        {
            return new ExpressionMapQueryPart(MapOperationType.Or, predicate);
        }

        #endregion
    }
}
