using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using PersistanceMap.QueryBuilder.Decorators;

namespace PersistanceMap
{
    /// <summary>
    /// Extension methods for type to create QueryParts
    /// </summary>
    internal static class TypeExtensionsForQueryParts
    {
        [Obsolete("Change from extension method to factory method or helper!", false)]
        public static EntityQueryPart<T> ToFromQueryPart<T>(this Type type, IQueryPartsMap queryParts)
        {
            var entity = new EntityQueryPart<T>(type.Name);
            entity.MapOperationType = MapOperationType.From;

            queryParts.Add(entity);

            return entity;
        }

        [Obsolete("Change from extension method to factory method or helper!", false)]
        public static EntityQueryPart<T> ToFromQueryPart<T>(this Type type, IQueryPartsMap queryParts, IQueryMap[] parts)
        {
            var entity = type.ToFromQueryPart<T>(queryParts);

            // first set identifier
            parts.Where(p => p.MapOperationType == MapOperationType.Identifier)
                .ForEach(part => entity.Identifier = part.Expression.Compile().DynamicInvoke() as string);

            // set include
            parts.Where(p => p.MapOperationType == MapOperationType.Include).ForEach(part =>
            {
                if (part.MapOperationType == MapOperationType.Include)
                {
                    //fromPart.AddOperation(part);
                    var field = new FieldQueryPart(FieldHelper.ExtractPropertyName(part.Expression), string.IsNullOrEmpty(entity.Identifier) ? entity.Entity : entity.Identifier, entity.Entity)
                    {
                        MapOperationType = MapOperationType.Include
                    };

                    queryParts.Add(field);
                }
            });

            return entity;
        }

        [Obsolete("Change from extension method to factory method or helper!", false)]
        public static EntityQueryPart<TJoin> ToJoinQueryPart<TJoin, T>(this Type type, IQueryPartsMap queryParts, Expression<Func<TJoin, T, bool>> predicate)
        {
            var operation = new QueryMap(MapOperationType.JoinOn, predicate);

            var entity = new EntityQueryPart<TJoin>(type.Name, null, new List<IQueryMap> { operation });
            entity.MapOperationType = MapOperationType.Join;

            queryParts.Add(entity);

            return entity;
        }

        [Obsolete("Change from extension method to factory method or helper!", false)]
        public static EntityQueryPart<TJoin> ToJoinQueryPart<TJoin, T>(this Type type, IQueryPartsMap queryParts, IQueryMap[] parts)
        {
            var operationParts = parts.Where(p => p.MapOperationType == MapOperationType.JoinOn || p.MapOperationType == MapOperationType.AndOn || p.MapOperationType == MapOperationType.OrOn).ToList();
            
            var entity = new EntityQueryPart<TJoin>(type.Name, null, operationParts)
            {
                MapOperationType = MapOperationType.Join
            };

            queryParts.Add(entity);

            // first set identifier
            var id = parts.Where(p => p.MapOperationType == MapOperationType.Identifier).Reverse().FirstOrDefault();
            if (id != null)
            {
                entity.Identifier = id.Expression.Compile().DynamicInvoke() as string;
                if (!string.IsNullOrEmpty(entity.Identifier))
                {
                    foreach (var part in entity.Operations)
                    {
                        part.IdentifierMap.Add(typeof(TJoin), entity.Identifier);
                    }
                }
            }

            // set include
            parts.Where(p => p.MapOperationType == MapOperationType.Include).ForEach(part =>
            {
                if (part.MapOperationType == MapOperationType.Include)
                {
                    //fromPart.AddOperation(part);
                    var field = new FieldQueryPart(FieldHelper.ExtractPropertyName(part.Expression), string.IsNullOrEmpty(entity.Identifier) ? entity.Entity : entity.Identifier, entity.Entity)
                    {
                        MapOperationType = MapOperationType.Include
                    };

                    queryParts.Add(field);
                }
            });

            return entity;
        }
    }
}
