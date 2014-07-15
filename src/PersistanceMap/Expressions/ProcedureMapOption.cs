using PersistanceMap.QueryBuilder;
using System;
using System.Linq.Expressions;

namespace PersistanceMap.Expressions
{
    /// <summary>
    /// MapOption for procedures
    /// </summary>
    public class ProcedureMapOption
    {
        //public IExpressionMapQueryPart Name(Expression<Func<string>> predicate)
        //{
        //    return new ExpressionMapQueryPart(MapOperationType.Identifier, predicate);
        //}

        public IMapQueryPart Value<T>(Expression<Func<T>> predicate)
        {
            return new MapQueryPart(MapOperationType.Value, predicate);
        }

        public IMapQueryPart Value<T>(string name, Expression<Func<T>> predicate)
        {
            // parameters have to start with @
            if (!name.StartsWith("@"))
                name = string.Format("@{0}", name);

            return new NamedMapQueryPart(MapOperationType.Value, name, predicate);
        }

        //public IMapQueryPart Value<T>(string name, Expression<Func<T>> predicate, Expression<Action<T>> callback)
        //{
        //    // parameters have to start with @
        //    if (!name.StartsWith("@"))
        //        name = string.Format("@{0}", name);

        //    return new CallbackMapQueryPart<T>(MapOperationType.Value, name, predicate, callback);
        //}
    }
}
