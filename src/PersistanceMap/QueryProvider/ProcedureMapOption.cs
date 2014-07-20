using PersistanceMap.QueryBuilder;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.QueryProvider
{
    /// <summary>
    /// MapOption for procedures
    /// </summary>
    public class ParameterMapOption
    {
        public IQueryMap Value<T>(Expression<Func<T>> predicate)
        {
            return new ParameterQueryMap(MapOperationType.Value, predicate);
        }

        public IQueryMap Value<T>(string name, Expression<Func<T>> predicate)
        {
            // parameters have to start with @
            if (!name.StartsWith("@"))
                name = string.Format("@{0}", name);

            return new ParameterQueryMap(MapOperationType.Value, name, predicate);
        }
    }
}
