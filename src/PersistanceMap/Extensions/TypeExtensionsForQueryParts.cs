using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace PersistanceMap
{
    /// <summary>
    /// Extension methods for type to create QueryParts
    /// </summary>
    internal static class TypeExtensionsForQueryParts
    {
        public static FromQueryPart ToFromQueryPart(this Type type)
        {
            return new FromQueryPart(type.Name);
        }

        public static JoinQueryPart<TJoin> ToJoinQueryPart<TJoin, T>(this Type type, Expression<Func<TJoin, T, bool>> predicate)
        {
            var operation = new ExpressionMapQueryPart(MapOperationType.Join, predicate);

            return new JoinQueryPart<TJoin>(type.Name, new List<IExpressionMapQueryPart> { operation });
        }

        public static JoinQueryPart<TJoin> ToJoinQueryPart<TJoin, T>(this Type type, IEnumerable<IExpressionMapQueryPart> parts)
        {
            IEnumerable<IExpressionMapQueryPart> operationParts = parts != null ? parts.Where(p => p.MapOperationType == MapOperationType.Join || p.MapOperationType == MapOperationType.And || p.MapOperationType == MapOperationType.Or).ToList() : null;
            IEnumerable<IExpressionMapQueryPart> idParts = parts != null ? parts.Where(p => p.MapOperationType == MapOperationType.Identifier).Reverse().ToList() : null;

            var join = new JoinQueryPart<TJoin>(type.Name, operationParts);
            if (idParts != null && idParts.Any())
            {
                join.Identifier = idParts.First().Expression.Compile().DynamicInvoke() as string;
            }

            return join;
        }
    }
}
