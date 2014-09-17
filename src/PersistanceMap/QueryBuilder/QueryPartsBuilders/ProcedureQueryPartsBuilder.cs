using PersistanceMap.QueryParts;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.QueryPartsBuilders
{
    internal class ProcedureQueryPartsBuilder : QueryPartsBuilder
    {
        protected ProcedureQueryPartsBuilder()
        {
        }

        private static ProcedureQueryPartsBuilder _instance;

        /// <summary>
        /// Gets the Singleton instance of the ProcedureQueryPartsBuilder
        /// </summary>
        public static ProcedureQueryPartsBuilder Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ProcedureQueryPartsBuilder();

                return _instance;
            }
        }

        public IParameterQueryPart AppendParameterQueryPart<T>(IQueryPartsMap queryParts, Expression<Func<T>> predicate, string name = null)
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

        public IParameterQueryPart AppendParameterQueryPart<T>(IQueryPartsMap queryParts, IExpressionQueryPart map, Action<T> callback)
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
