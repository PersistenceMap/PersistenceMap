using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryBuilder.Decorators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceMap.QueryProvider
{
    public partial class SelectQueryProvider<T> : IJoinQueryProvider<T>
    {
        #region IJoinQueryProvider Implementation

        //public IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate)
        //{
        //    AppendExpressionQueryPartToLast(OperationType.And, predicate);

        //    return new SelectQueryProvider<T>(Context, QueryPartsMap);
        //}

        public IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null)
        {
            var part = AppendExpressionQueryPartToLast(OperationType.And, predicate);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(source))
                part.AliasMap.Add(typeof(T), source);

            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(TAnd), alias);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        public IJoinQueryProvider<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate)
        {
            AppendExpressionQueryPartToLast(OperationType.Or, predicate);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        #endregion
    }
}
