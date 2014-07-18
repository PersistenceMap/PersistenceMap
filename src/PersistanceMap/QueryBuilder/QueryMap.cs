using PersistanceMap.Compiler;
using PersistanceMap.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    internal class QueryMap : IQueryMap, IQueryPart
    {
        public QueryMap(MapOperationType operationtype, LambdaExpression expression)
        {
            MapOperationType = operationtype;
            Expression = expression;

            IdentifierMap = new Dictionary<Type, string>();
        }

        public MapOperationType MapOperationType { get; private set; }

        public LambdaExpression Expression { get; private set; }

        public Dictionary<Type, string> IdentifierMap { get; private set; }

        public virtual string Compile()
        {
            var value = LambdaExpressionToSqlCompiler.Instance.Compile(this/*Expression*/);
            if (value != null)
                return DialectProvider.Instance.GetQuotedValue(value, value.GetType());

            return null;
        }

        internal void AddIdentifier(Type type, string identifier)
        {
            if (!string.IsNullOrEmpty(identifier))
                IdentifierMap.Add(type, identifier);
        }
    }
}
