using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryProvider
{
    public partial class SelectQueryProvider<T> : IJoinQueryProvider<T>
    {
        #region IJoinQueryProvider Implementation

        public IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null)
        {
            return And<T, TAnd>(predicate, alias, source);
        }

        public IJoinQueryProvider<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate, string alias = null, string source = null)
        {
            return Or<T, TOr>(predicate, alias, source);
        }

        #endregion
    }
}
