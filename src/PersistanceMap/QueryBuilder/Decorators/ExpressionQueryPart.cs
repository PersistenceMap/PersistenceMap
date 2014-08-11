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

        //public OperationType OperationType { get; private set; }

        public LambdaExpression Expression { get; private set; }

        /// <summary>
        /// Defines a mapping for types and the alias that the entity has
        /// </summary>
        public Dictionary<Type, string> AliasMap { get; private set; }

        public override string Compile()
        {
            var value = LambdaExpressionToSqlCompiler.Instance.Compile(this);
            if (value == null)
                return null;

            string prefix = string.Empty;
            string sufix = string.Empty;
            switch (OperationType)
            {
                case OperationType.On:
                    prefix = "on";
                    break;

                case OperationType.And:
                    prefix = "and";
                    break;

                case OperationType.Or:
                    prefix = "or";
                    break;

                case OperationType.Value:
                    // just return the quotated value
                    prefix = string.Empty;
                    break;

                case OperationType.Where:
                    prefix = "where";
                    break;

                case OperationType.OrderBy:
                    prefix = "order by";
                    sufix = "asc";
                    break;

                case OperationType.OrderByDesc:
                    prefix = "order by";
                    sufix = "desc";
                    break;

                case OperationType.ThenBy:
                    // just return the quotated value
                    prefix = ",";
                    sufix = "asc";
                    break;

                case OperationType.ThenByDesc:
                    // just return the quotated value
                    prefix = ",";
                    sufix = "desc";
                    break;
            }

            var sb = new StringBuilder();
            string expression = string.Empty;

            if (!string.IsNullOrEmpty(prefix))
            {
                //sb.AppendLine(string.Format("{0} {1} ", prefix, value));
                expression = string.Format("{0} {1} ", prefix, value);
            }
            else
            {
                //sb.AppendLine(DialectProvider.Instance.GetQuotedValue(value, value.GetType()));
                expression = DialectProvider.Instance.GetQuotedValue(value, value.GetType());
            }

            if (!string.IsNullOrEmpty(sufix))
                expression = string.Format("{0} {1} ", expression.TrimEnd(), sufix);

            //TODO: AppendLine or just append? Check what should happen when a child part is added!
            sb.Append(expression);

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
