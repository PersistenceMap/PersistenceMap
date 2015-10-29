using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public partial class JoinQueryBuilder<T> : SelectQueryBuilder<T>, IJoinQueryExpression<T>
    {
        public JoinQueryBuilder(IDatabaseContext context) 
            : base(context)
        {
        }

        public JoinQueryBuilder(IDatabaseContext context, SelectQueryPartsContainer container) 
            : base(context, container)
        {
        }

        #region IJoinQueryProvider Implementation
        
        public IJoinQueryExpression<T> And<TAnd>(Expression<Func<T, TAnd, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.And, () => LambdaToSqlCompiler.Compile(partMap), typeof(T));

            QueryParts.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
            {
                partMap.AliasMap.Add(typeof (T), alias);
            }

            if (!string.IsNullOrEmpty(source))
            {
                partMap.AliasMap.Add(typeof (TAnd), source);
            }

            return new JoinQueryBuilder<T>(Context, QueryParts);
        }
        
        public IJoinQueryExpression<T> Or<TOr>(Expression<Func<T, TOr, bool>> operation, string alias = null, string source = null)
        {
            var partMap = new ExpressionAliasMap(operation);
            var part = new DelegateQueryPart(OperationType.Or, () => LambdaToSqlCompiler.Compile(partMap), typeof(T));
            QueryParts.Add(part);

            // add aliases to mapcollections
            if (!string.IsNullOrEmpty(alias))
            {
                partMap.AliasMap.Add(typeof(T), alias);
            }

            if (!string.IsNullOrEmpty(source))
            {
                partMap.AliasMap.Add(typeof(TOr), source);
            }

            return new JoinQueryBuilder<T>(Context, QueryParts);
        }
        
        #endregion
    }
}
