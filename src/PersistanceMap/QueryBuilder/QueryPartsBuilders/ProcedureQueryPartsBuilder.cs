using PersistanceMap.QueryParts;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.QueryPartsBuilders
{
    public class ProcedureQueryPartsBuilder : QueryPartsBuilder
    {
        protected ProcedureQueryPartsBuilder()
        {
        }


        public static IParameterQueryPart AppendParameterQueryPart<T>(IQueryPartsMap queryParts, Expression<Func<T>> predicate, string name = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                // parameters have to start with @
                if (!name.StartsWith("@"))
                    name = string.Format("@{0}", name);
            }

            var part = new ParameterQueryPart(new IExpressionQueryPart[] { new ParameterQueryMap(OperationType.Value, name, predicate) })
            {
                OperationType = OperationType.Parameter
            };

            queryParts.Add(part);

            return part;
        }

        public static IParameterQueryPart AppendParameterQueryPart<T>(IQueryPartsMap queryParts, IExpressionQueryPart map, Action<T> callback)
        {
            var part = new CallbackParameterQueryPart<T>(new IExpressionQueryPart[] { map }, callback)
            {
                OperationType = OperationType.Parameter
            };

            queryParts.Add(part);

            return part;
        }
    }
}
