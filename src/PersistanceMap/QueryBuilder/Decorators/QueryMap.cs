using PersistanceMap.Compiler;
using PersistanceMap.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.Decorators
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
            var value = LambdaExpressionToSqlCompiler.Instance.Compile(this);
            if (value == null)
                return null;

            string keyword = string.Empty;
            switch (MapOperationType)
            {
                case MapOperationType.JoinOn:
                    keyword = "on";
                    break;

                case MapOperationType.AndOn:
                    keyword = "and";
                    break;

                case MapOperationType.OrOn:
                    keyword = "or";
                    break;

                case MapOperationType.Value:
                    // just return the quotated value
                    keyword = string.Empty;
                    break;
            }


            if (!string.IsNullOrEmpty(keyword))
                return string.Format("{0} {1}", keyword, value);

            return DialectProvider.Instance.GetQuotedValue(value, value.GetType());
        }

        internal void AddIdentifier(Type type, string identifier)
        {
            if (!string.IsNullOrEmpty(identifier))
                IdentifierMap.Add(type, identifier);
        }
    }
}
