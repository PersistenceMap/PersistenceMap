using PersistanceMap.Compiler;
using PersistanceMap.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.Decorators
{
    internal class QueryMap : IQueryMap, IQueryPart
    {
        public QueryMap(OperationType operationtype, LambdaExpression expression)
        {
            OperationType = operationtype;
            Expression = expression;

            AliasMap = new Dictionary<Type, string>();
        }

        public OperationType OperationType { get; private set; }

        public LambdaExpression Expression { get; private set; }

        /// <summary>
        /// Defines a mapping for types and the alias that the entity has
        /// </summary>
        public Dictionary<Type, string> AliasMap { get; private set; }

        public virtual string Compile()
        {
            var value = LambdaExpressionToSqlCompiler.Instance.Compile(this);
            if (value == null)
                return null;

            string keyword = string.Empty;
            switch (OperationType)
            {
                case OperationType.JoinOn:
                    keyword = "on";
                    break;

                case OperationType.AndOn:
                    keyword = "and";
                    break;

                case OperationType.OrOn:
                    keyword = "or";
                    break;

                case OperationType.Value:
                    // just return the quotated value
                    keyword = string.Empty;
                    break;
            }


            if (!string.IsNullOrEmpty(keyword))
                return string.Format("{0} {1} ", keyword, value);

            return DialectProvider.Instance.GetQuotedValue(value, value.GetType());
        }

        internal void AddEntityAlias(Type type, string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                AliasMap.Add(type, alias);
        }
    }
}
