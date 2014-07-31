using PersistanceMap.Sql;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.Decorators
{
    internal class ParameterQueryMap : ExpressionQueryPart, INamedQueryPart
    {
        public ParameterQueryMap(OperationType operationtype, LambdaExpression expression)
            : this(operationtype, null, expression)
        {
        }

        public ParameterQueryMap(OperationType operationtype, string name, LambdaExpression expression)
            : base(operationtype, expression)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the parameter
        /// </summary>
        public string Name { get; private set; }

        public override string Compile()
        {
            // get the value. Dont compile the expression to sql
            var value = Expression.Compile().DynamicInvoke();
            if (value != null)
            {
                // quotate and format the value if needed
                var quotated = DialectProvider.Instance.GetQuotedValue(value, value.GetType());

                // return only the formated value if the parameter has no name
                if (string.IsNullOrEmpty(Name) && quotated != null)
                {
                    return quotated;
                }

                // return the name with the formated value
                if (quotated != null)
                    return string.Format("{0}={1}", Name, quotated);

                // return the name with the unformated value
                return string.Format("{0}={1}", Name, value);
            }

            // return the name with the compiled expression
            if (string.IsNullOrEmpty(Name))
                return string.Format("{0}={1}", Name, base.Compile());

            // compile the expression if there is no value and no name
            return base.Compile();
        }
    }
}
