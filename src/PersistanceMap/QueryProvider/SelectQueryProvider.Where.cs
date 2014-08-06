using PersistanceMap.QueryBuilder.Decorators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryProvider
{
    public partial class SelectQueryProvider<T> : IWhereQueryProvider<T>, IQueryProvider
    {
        #region IWhereQueryProvider Implementation

        #region And Expressions

        public IJoinQueryProvider<T> And(Expression<Func<T, bool>> predicate)
        {
            return AddExpressionQueryPartToLast(OperationType.And, predicate);
        }

        public IJoinQueryProvider<T> And<TAnd>(Expression<Func<TAnd, bool>> predicate)
        {
            return AddExpressionQueryPartToLast(OperationType.And, predicate);
        }
        
        IJoinQueryProvider<T> IWhereQueryProvider<T>.And<TAnd>(Expression<Func<T, TAnd, bool>> predicate)
        {
            return And<TAnd>(predicate);
        }

        public IJoinQueryProvider<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> predicate)
        {
            return AddExpressionQueryPartToLast(OperationType.And, predicate);
        }

        #endregion

        #region Or Expressions

        public IJoinQueryProvider<T> Or(Expression<Func<T, bool>> predicate)
        {
            return AddExpressionQueryPartToLast(OperationType.Or, predicate);
        }

        public IJoinQueryProvider<T> Or<TOr>(Expression<Func<TOr, bool>> predicate)
        {
            return AddExpressionQueryPartToLast(OperationType.Or, predicate);
        }

        IJoinQueryProvider<T> IWhereQueryProvider<T>.Or<TOr>(Expression<Func<T, TOr, bool>> predicate)
        {
            return Or<TOr>(predicate);
        }

        public IJoinQueryProvider<T> Or<TSource, TOr>(Expression<Func<TSource, TOr, bool>> predicate)
        {
            return AddExpressionQueryPartToLast(OperationType.Or, predicate);
        }

        #endregion

        #region OrderBy Expressions

        IOrderQueryProvider<T> IWhereQueryProvider<T>.OrderBy<TOrder>(Expression<Func<T, TOrder>> predicate)
        {
            return OrderBy<TOrder>(predicate);
        }

        IOrderQueryProvider<T2> IWhereQueryProvider<T>.OrderBy<T2, TOrder>(Expression<Func<T2, TOrder>> predicate)
        {
            return OrderBy<T2, TOrder>(predicate);
        }

        IOrderQueryProvider<T> IWhereQueryProvider<T>.OrderByDesc<TOrder>(Expression<Func<T, TOrder>> predicate)
        {
            return OrderByDesc<TOrder>(predicate);
        }

        IOrderQueryProvider<T2> IWhereQueryProvider<T>.OrderByDesc<T2, TOrder>(Expression<Func<T2, TOrder>> predicate)
        {
            return OrderByDesc<T2, TOrder>(predicate);
        }
        
        #endregion

        #endregion
    }
}