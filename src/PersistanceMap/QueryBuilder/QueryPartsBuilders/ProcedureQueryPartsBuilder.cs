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

        private static ProcedureQueryPartsBuilder instance;

        /// <summary>
        /// Gets the Singleton instance of the ProcedureQueryPartsBuilder
        /// </summary>
        public static new ProcedureQueryPartsBuilder Instance
        {
            get
            {
                if (instance == null)
                    instance = new ProcedureQueryPartsBuilder();

                return instance;
            }
        }

        //public IParameterQueryPart AppendParameterQueryPart<T>(IQueryPartsMap queryParts, Expression<Func<T>> predicate, string name = null)
        //{
        //    if (!string.IsNullOrEmpty(name))
        //    {
        //        // parameters have to start with @
        //        if (!name.StartsWith("@"))
        //            name = string.Format("@{0}", name);
        //    }

        //    var part = new ParameterQueryPart(new IQueryPart[] { new ParameterQueryMap(OperationType.Value, name, predicate) })
        //    {
        //        OperationType = OperationType.Parameter
        //    };

        //    queryParts.Add(part);

        //    return part;
        //}

        //public IParameterQueryPart AppendParameterQueryPart<T>(IQueryPartsMap queryParts, IQueryPart map, Action<T> callback)
        //{
        //    var part = new CallbackParameterQueryPart<T>(new IQueryPart[] { map }, callback)
        //    {
        //        OperationType = OperationType.Parameter
        //    };

        //    queryParts.Add(part);

        //    return part;
        //}
    }
}
