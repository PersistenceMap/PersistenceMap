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
            AppendExpressionQueryPartToLast(OperationType.And, predicate);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        public IJoinQueryProvider<T> And<TAnd>(Expression<Func<TAnd, bool>> predicate, string alias = null)
        {
            var part = AppendExpressionQueryPartToLast(OperationType.And, predicate);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(TAnd), alias);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        //IJoinQueryProvider<T> IWhereQueryProvider<T>.And<TAnd>(Expression<Func<T, TAnd, bool>> predicate, string alias = null, string source = null)
        //{
        //    return And<TAnd>(predicate, alias, source);
        //}

        public IJoinQueryProvider<T> And<TSource, TAnd>(Expression<Func<TSource, TAnd, bool>> predicate, string alias = null, string source = null)
        {
            var part = AppendExpressionQueryPartToLast(OperationType.And, predicate);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(source))
                part.AliasMap.Add(typeof(TAnd), source);

            if (!string.IsNullOrEmpty(alias))
                part.AliasMap.Add(typeof(TSource), alias);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        #endregion

        #region Or Expressions

        public IJoinQueryProvider<T> Or(Expression<Func<T, bool>> predicate)
        {
            AppendExpressionQueryPartToLast(OperationType.Or, predicate);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        public IJoinQueryProvider<T> Or<TOr>(Expression<Func<TOr, bool>> predicate)
        {
            AppendExpressionQueryPartToLast(OperationType.Or, predicate);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        IJoinQueryProvider<T> IWhereQueryProvider<T>.Or<TOr>(Expression<Func<T, TOr, bool>> predicate)
        {
            return Or<TOr>(predicate);
        }

        public IJoinQueryProvider<T> Or<TSource, TOr>(Expression<Func<TSource, TOr, bool>> predicate)
        {
            AppendExpressionQueryPartToLast(OperationType.Or, predicate);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
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