using PersistanceMap.Compiler;
using PersistanceMap.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PersistanceMap.QueryBuilder.Decorators
{
    internal class ExpressionQueryPart : QueryPartDecorator, IExpressionQueryPart, IQueryPartDecorator, IQueryPart
    {
        public ExpressionQueryPart(OperationType operationtype, LambdaExpression expression)
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
                case OperationType.On:
                    keyword = "on";
                    break;

                case OperationType.And:
                    keyword = "and";
                    break;

                case OperationType.Or:
                    keyword = "or";
                    break;

                case OperationType.Value:
                    // just return the quotated value
                    keyword = string.Empty;
                    break;

                case OperationType.Where:
                    keyword = "where";
                    break;
            }

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(keyword))
            {
                sb.AppendLine(string.Format("{0} {1} ", keyword, value));
            }
            else
                sb.AppendLine(DialectProvider.Instance.GetQuotedValue(value, value.GetType()));


            var partsValue = base.Compile();
            if (!string.IsNullOrEmpty(partsValue))
                sb.AppendLine(partsValue);

            return sb.ToString().RemoveLineBreak();
        }

        internal void AddEntityAlias(Type type, string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                AliasMap.Add(type, alias);
        }

        public override string ToString()
        {
            if (Expression == null)
                return string.Format("Expression with Operation: [{0}]", OperationType);

            return string.Format("Expression: [{0}] Operation: [{1}]", Expression, OperationType);
        }
    }
}
