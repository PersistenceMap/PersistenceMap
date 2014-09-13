using PersistanceMap.QueryParts;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.QueryPartsBuilders
{
    public class SelectQueryPartsBuilder<T> : QueryPartsBuilder
    {
        protected SelectQueryPartsBuilder()
        {
        }

        

        /// <summary>
        /// Creates an ExpressionQueryPart and adds it to the last Join, LeftJoin, FullJoin, RightJoin or Where operation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="queryPartsMap"></param>
        /// <param name="operation">The operationtype for the new querypart</param>
        /// <param name="predicate">The predicate containing the expression to execute</param>
        /// <returns>A new instance of selectqueryprovider containing a cumlated view of the complete expression</returns>
        protected IExpressionQueryPart AppendExpressionQueryPartToLast(IDatabaseContext context, SelectQueryPartsMap queryPartsMap, OperationType operation, LambdaExpression predicate)
        {
            var part = new ExpressionQueryPart(operation, predicate);
            queryPartsMap.AddToLast(part, p => p.OperationType == OperationType.Join || p.OperationType == OperationType.LeftJoin || p.OperationType == OperationType.FullJoin || p.OperationType == OperationType.RightJoin || p.OperationType == OperationType.Where);

            return part;
        }

        protected SelectQueryBuilder<T2> CreateExpressionQueryPart<T2>(IDatabaseContext context, SelectQueryPartsMap queryPartsMap, OperationType operation, LambdaExpression predicate)
        {
            AppendExpressionQueryPart(queryPartsMap, predicate, operation);

            return new SelectQueryBuilder<T2>(context, queryPartsMap);
        }

        protected IJoinQueryProvider<T1> CreateEntityQueryPart<T1, T2>(IDatabaseContext context, SelectQueryPartsMap queryPartsMap, Expression<Func<T1, T2, bool>> predicate, OperationType operation, string alias = null, string source = null)
        {
            var part = AppendEntityQueryPart(queryPartsMap, predicate, operation);
            if (!string.IsNullOrEmpty(alias))
                part.EntityAlias = alias;

            foreach (var itm in part.Parts)
            {
                var map = itm as IExpressionQueryPart;
                if (map == null)
                    continue;

                // add aliases to mapcollections
                if (!string.IsNullOrEmpty(source))
                    map.AliasMap.Add(typeof(T2), source);

                if (!string.IsNullOrEmpty(alias))
                    map.AliasMap.Add(typeof(T1), alias);
            }

            return new SelectQueryBuilder<T1>(context, queryPartsMap);
        }
    }
}
