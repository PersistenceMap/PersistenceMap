using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.QueryParts
{
    /// <summary>
    /// Defines a Interface that is used to add alias mappings to a expression. This gets used when compiling the expression to a sql string
    /// </summary>
    public interface IExpressionMap
    {
        /// <summary>
        /// The expression that contains alias mappings
        /// </summary>
        LambdaExpression Expression { get; }

        /// <summary>
        /// Defines a mapping for types and the alias that the entity has
        /// </summary>
        Dictionary<Type, string> AliasMap { get; }
    }
}
