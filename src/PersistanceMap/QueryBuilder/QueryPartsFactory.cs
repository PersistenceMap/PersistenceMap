using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryBuilder.Decorators;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap
{
    /// <summary>
    /// Extension methods for type to create QueryParts
    /// </summary>
    internal static class QueryPartsFactory
    {
        public static EntityQueryPart<T> CreateEntityQueryPart<T>(IQueryPartsMap queryParts, MapOperationType maptype)
        {
            var type = typeof(T);
            var entity = new EntityQueryPart<T>(type.Name)
            {
                MapOperationType = maptype
            };

            queryParts.Add(entity);

            return entity;
        }

        public static EntityQueryPart<T> CreateEntityQueryPart<T, T2>(IQueryPartsMap queryParts, Expression<Func<T, T2, bool>> predicate, MapOperationType maptype)
        {
            var operation = new QueryMap(MapOperationType.JoinOn, predicate);

            return CreateEntityQueryPart<T>(queryParts, new IQueryMap[] { operation }, maptype);
        }

        public static EntityQueryPart<T> CreateEntityQueryPart<T>(IQueryPartsMap queryParts, IQueryMap[] parts, MapOperationType maptype)
        {
            var operationParts = parts.Where(p => p.MapOperationType == MapOperationType.JoinOn || p.MapOperationType == MapOperationType.AndOn || p.MapOperationType == MapOperationType.OrOn).ToArray();

            var type = typeof(T);
            var entity = new EntityQueryPart<T>(type.Name, null, operationParts)
            {
                MapOperationType = maptype
            };

            queryParts.Add(entity);

            // first set identifier
            var id = parts.Where(p => p.MapOperationType == MapOperationType.Identifier).Reverse().FirstOrDefault();
            if (id != null)
            {
                entity.Identifier = id.Expression.Compile().DynamicInvoke() as string;
                if (!string.IsNullOrEmpty(entity.Identifier))
                {
                    foreach (var part in entity.MapCollection)
                    {
                        part.IdentifierMap.Add(typeof(T), entity.Identifier);
                    }
                }
            }

            // set include
            parts.Where(p => p.MapOperationType == MapOperationType.Include).ForEach(part =>
            {
                //if (part.MapOperationType == MapOperationType.Include)
                //{
                    var field = new FieldQueryPart(FieldHelper.ExtractPropertyName(part.Expression), string.IsNullOrEmpty(entity.Identifier) ? entity.Entity : entity.Identifier, entity.Entity)
                    {
                        MapOperationType = MapOperationType.Include
                    };

                    queryParts.Add(field);
                //}
            });

            return entity;
        }
    }
}
