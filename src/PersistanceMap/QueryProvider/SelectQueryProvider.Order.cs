using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryProvider
{
    public partial class SelectQueryProvider<T> : IOrderQueryProvider<T>, ISelectQueryExpression<T>, IQueryProvider
    {
        #region OrderBy Expressions

        public IOrderQueryProvider<T> ThenBy<TOrder>(Expression<Func<T, TOrder>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.ThenBy, predicate);
        }

        public IOrderQueryProvider<T> ThenBy<T2, TOrder>(Expression<Func<T2, TOrder>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.ThenBy, predicate);
        }

        public IOrderQueryProvider<T> ThenByDesc<TOrder>(Expression<Func<T, TOrder>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.ThenByDesc, predicate);
        }

        public IOrderQueryProvider<T> ThenByDesc<T2, TOrder>(Expression<Func<T2, TOrder>> predicate)
        {
            return CreateExpressionQueryPart<T>(OperationType.ThenByDesc, predicate);
        }

        #endregion
    }
}
