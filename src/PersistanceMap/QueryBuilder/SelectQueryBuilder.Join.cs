using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IJoinQueryExpression<T>
    {
        #region IJoinQueryProvider Implementation

        IJoinQueryExpression<T> IJoinQueryExpression<T>.And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null)
        {
            return And<TAnd>(predicate, alias, source);
        }

        IJoinQueryExpression<T> IJoinQueryExpression<T>.Or<TOr>(Expression<Func<T, TOr, bool>> predicate, string alias = null, string source = null)
        {
            return Or<TOr>(predicate, alias, source);
        }

        #endregion
    }
}
