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

        public IJoinQueryProvider<T> And(Expression<Func<T, bool>> predicate)
        {
            return AddExpressionPartToLast(OperationType.And, predicate);
        }

        public IJoinQueryProvider<T> And<TAnd>(Expression<Func<TAnd, bool>> predicate)
        {
            return AddExpressionPartToLast(OperationType.And, predicate);
        }
        
        IJoinQueryProvider<T> IWhereQueryProvider<T>.And<TAnd>(Expression<Func<T, TAnd, bool>> predicate)
        {
            return And<TAnd>(predicate);
        }

        public IJoinQueryProvider<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> predicate)
        {
            return AddExpressionPartToLast(OperationType.And, predicate);
        }


        public IJoinQueryProvider<T> Or(Expression<Func<T, bool>> predicate)
        {
            return AddExpressionPartToLast(OperationType.Or, predicate);
        }

        public IJoinQueryProvider<T> Or<TOr>(Expression<Func<TOr, bool>> predicate)
        {
            return AddExpressionPartToLast(OperationType.Or, predicate);
        }

        IJoinQueryProvider<T> IWhereQueryProvider<T>.Or<TOr>(Expression<Func<T, TOr, bool>> predicate)
        {
            return Or<TOr>(predicate);
        }

        public IJoinQueryProvider<T> Or<TSource, TOr>(Expression<Func<TSource, TOr, bool>> predicate)
        {
            return AddExpressionPartToLast(OperationType.Or, predicate);
        }
    
        #endregion
    }
}