using PersistanceMap.QueryParts;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.QueryPartsBuilders
{
    internal class SelectQueryPartsBuilder : QueryPartsBuilder
    {
        protected SelectQueryPartsBuilder()
        {
        }

        private static SelectQueryPartsBuilder _instance;

        /// <summary>
        /// Gets the Singleton instance of the QueryPartsBuilder
        /// </summary>
        public static SelectQueryPartsBuilder Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SelectQueryPartsBuilder();

                return _instance;
            }
        }

        /// <summary>
        /// Creates an ExpressionQueryPart and adds it to the last Join, LeftJoin, FullJoin, RightJoin or Where operation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="queryPartsMap"></param>
        /// <param name="operation">The operationtype for the new querypart</param>
        /// <param name="predicate">The predicate containing the expression to execute</param>
        /// <returns>A new instance of selectqueryprovider containing a cumlated view of the complete expression</returns>
        internal IExpressionQueryPart AppendExpressionQueryPartToLast(IDatabaseContext context, SelectQueryPartsMap queryPartsMap, OperationType operation, LambdaExpression predicate)
        {
            var part = new ExpressionQueryPart(operation, predicate);
            queryPartsMap.AddToLast(part, p => p.OperationType == OperationType.Join || p.OperationType == OperationType.LeftJoin || p.OperationType == OperationType.FullJoin || p.OperationType == OperationType.RightJoin || p.OperationType == OperationType.Where);

            return part;
        }
    }
}
