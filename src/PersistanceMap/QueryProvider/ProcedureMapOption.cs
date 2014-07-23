using PersistanceMap.QueryBuilder;
using System;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder.Decorators;

namespace PersistanceMap.QueryProvider
{
    /// <summary>
    /// MapOption for procedures
    /// </summary>
    public class ProcedureMapOption
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

    public class ProcedureMapOption<T>
    {
        public IQueryMap MapTo<TOut>(string source, Expression<Func<T, TOut>> alias)
        {
            throw new NotImplementedException();

            //return new PredicateQueryPart(MapOperationType.Include,
            //    () =>
            //    {
            //        return string.Format("{0} as {1}", source, FieldHelper.ExtractPropertyName(alias));
            //    });
        }
    }
}
