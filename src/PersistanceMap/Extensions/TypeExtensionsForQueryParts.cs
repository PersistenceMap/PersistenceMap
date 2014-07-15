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
        public static FromQueryPart<T> ToFromQueryPart<T>(this Type type)
        {
            return new FromQueryPart<T>(type.Name);
        }

        public static JoinQueryPart<TJoin> ToJoinQueryPart<TJoin, T>(this Type type, Expression<Func<TJoin, T, bool>> predicate)
        {
            var operation = new MapQueryPart(MapOperationType.Join, predicate);

            return new JoinQueryPart<TJoin>(type.Name, new List<IMapQueryPart> { operation });
        }

        public static JoinQueryPart<TJoin> ToJoinQueryPart<TJoin, T>(this Type type, IEnumerable<IMapQueryPart> parts)
        {
            IEnumerable<IMapQueryPart> operationParts = parts != null ? parts.Where(p => p.MapOperationType == MapOperationType.Join || p.MapOperationType == MapOperationType.And || p.MapOperationType == MapOperationType.Or).ToList() : null;
            IEnumerable<IMapQueryPart> idParts = parts != null ? parts.Where(p => p.MapOperationType == MapOperationType.Identifier).Reverse().ToList() : null;
            IEnumerable<IMapQueryPart> includeParts = parts != null ? parts.Where(p => p.MapOperationType == MapOperationType.Include).ToList() : null;

            var join = new JoinQueryPart<TJoin>(type.Name, operationParts);
            if (includeParts != null)
                join.AddOperations(includeParts);

            if (idParts != null && idParts.Any())
            {
                join.Identifier = idParts.First().Expression.Compile().DynamicInvoke() as string;
                if (!string.IsNullOrEmpty(join.Identifier))
                {
                    foreach (var part in join.Operations)
                    {
                        var idpart = part as IIdentifierMapQueryPart;
                        if (idpart != null)
                            idpart.IdentifierMap.Add(typeof(TJoin), join.Identifier);
                    }
                }
            }

            return join;
        }
    }
}
