using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using System;
using System.Linq.Expressions;
using PersistanceMap.QueryBuilder.Decorators;

namespace PersistanceMap.QueryProvider
{
    /// <summary>
    /// MapOption for procedures
    /// </summary>
    internal class ProcedureMapOption : IProcedureMapOption
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

    internal class ProcedureMapOption<T> : IProcedureMapOption<T>
    {
        public IQueryMap MapTo<TOut>(string source, Expression<Func<T, TOut>> alias)
        {
            var aliasField = FieldHelper.TryExtractPropertyName(alias);

            // create a new expression that returns the field with a alias
            var entity = typeof(T).Name;
            return new FieldQueryPart(source, aliasField, null /*EntityAlias*/, entity/*, expression*/)
            {
                MapOperationType = MapOperationType.Include
            };
        }
    }
}
