using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : IJoinQueryExpression<T>
    {
        #region IJoinQueryProvider Implementation

        IJoinQueryExpression<T> IJoinQueryExpression<T>.And<TAnd>(Expression<Func<T, TAnd, bool>> operation, string alias = null, string source = null)
        {
            return And<TAnd>(operation, alias, source);
        }

        IJoinQueryExpression<T> IJoinQueryExpression<T>.Or<TOr>(Expression<Func<T, TOr, bool>> operation, string alias = null, string source = null)
        {
            return Or<TOr>(operation, alias, source);
        }

        #endregion
    }
}
