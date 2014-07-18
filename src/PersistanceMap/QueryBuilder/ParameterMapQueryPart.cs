using PersistanceMap.Sql;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    internal class ParameterMapQueryPart : MapQueryPart, INamedQueryPart
    {
        public ParameterMapQueryPart(MapOperationType operationtype, LambdaExpression expression)
            : this(operationtype, null, expression)
        {
        }

        public ParameterMapQueryPart(MapOperationType operationtype, string name, LambdaExpression expression)
            : base(operationtype, expression)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override string Compile()
        {
            // get the value. Dont compile the expression to sql
            var value = Expression.Compile().DynamicInvoke();
            if (value != null)
            {
                var quotated = DialectProvider.Instance.GetQuotedValue(value, value.GetType());

                if (string.IsNullOrEmpty(Name))
                {
                    if (quotated != null)
                        return quotated;
                }

                if (quotated != null)
                    return string.Format("{0}={1}", Name, quotated);

                return string.Format("{0}={1}", Name, value);
            }

            if (string.IsNullOrEmpty(Name))
                return string.Format("{0}={1}", Name, base.Compile());

            return base.Compile();
        }
    }
}
