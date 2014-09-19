using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IOrderQueryExpression<T>, ISelectQueryExpressionBase<T>, IQueryProvider
    {
        #region OrderBy Expressions

        public IOrderQueryExpression<T> ThenBy(Expression<Func<T, object>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.ThenBy, predicate);
        }

        public IOrderQueryExpression<T> ThenBy<T2>(Expression<Func<T2, object>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.ThenBy, predicate);
        }

        //public IOrderQueryProvider<T> ThenByDesc<TOrder>(Expression<Func<T, TOrder>> predicate)
        public IOrderQueryExpression<T> ThenByDesc(Expression<Func<T, object>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.ThenByDesc, predicate);
        }

        //public IOrderQueryProvider<T> ThenByDesc<T2, TOrder>(Expression<Func<T2, TOrder>> predicate)
        public IOrderQueryExpression<T> ThenByDesc<T2>(Expression<Func<T2, object>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.ThenByDesc, predicate);
        }

        #endregion
    }
}


