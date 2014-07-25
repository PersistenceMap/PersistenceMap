using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public interface IQueryMap : IQueryPart
    {
        LambdaExpression Expression { get; }

        /// <summary>
        /// Defines a mapping for types and the alias that the entity has
        /// </summary>
        Dictionary<Type, string> AliasMap { get; }
    }
}
