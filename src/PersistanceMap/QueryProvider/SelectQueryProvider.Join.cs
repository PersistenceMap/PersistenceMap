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

        public IJoinQueryProvider<T> And<TAnd>(Expression<Func<T, TAnd, bool>> predicate)
        {
            return AddExpressionPartToLast(OperationType.And, predicate);
        }

        public IJoinQueryProvider<T> And<TAnd>(string source, string reference, Expression<Func<T, TAnd, bool>> predicate)
        {
            var part = new ExpressionQueryPart(OperationType.And, predicate);
            QueryPartsMap.AddToLast(part, p => p.OperationType == OperationType.Join || p.OperationType == OperationType.LeftJoin || p.OperationType == OperationType.FullJoin || p.OperationType == OperationType.RightJoin || p.OperationType == OperationType.Where);

            // add aliases to mapcollections
            part.AliasMap.Add(typeof(T), source);
            part.AliasMap.Add(typeof(TAnd), reference);

            return new SelectQueryProvider<T>(Context, QueryPartsMap);
        }

        public IJoinQueryProvider<T> Or<TOr>(Expression<Func<T, TOr, bool>> predicate)
        {
            return AddExpressionPartToLast(OperationType.Or, predicate);
        }

        #endregion
    }
}
