using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryProvider
{
    public partial class SelectQueryProvider<T> : IJoinQueryProvider<T>
    {
        #region IJoinQueryProvider Implementation

        IJoinQueryProvider<T> IJoinQueryProvider<T>.And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null)
        {
            return And<TAnd>(predicate, alias, source);
        }

        IJoinQueryProvider<T> IJoinQueryProvider<T>.Or<TOr>(Expression<Func<T, TOr, bool>> predicate, string alias = null, string source = null)
        {
            return Or<TOr>(predicate, alias, source);
        }

        #endregion
    }
}
