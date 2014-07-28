using PersistanceMap.Compiler;
using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryBuilder.Decorators;
using System;
using System.Linq;
using System.Linq.Expressions;
using PersistanceMap.QueryProvider;

namespace PersistanceMap
{
    /// <summary>
    /// Extension methods for type to create QueryParts
    /// </summary>
    internal static class QueryPartsFactory
    {
        #region EntityQueryPart

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

            // first set alias
            var id = parts.Where(p => p.MapOperationType == MapOperationType.As).Reverse().FirstOrDefault();
            if (id != null)
            {
                entity.EntityAlias = id.Expression.Compile().DynamicInvoke() as string;
                if (!string.IsNullOrEmpty(entity.EntityAlias))
                {
                    foreach (var part in entity.MapCollection)
                    {
                        part.AliasMap.Add(typeof(T), entity.EntityAlias);
                    }
                }
            }

            // set include
            parts.Where(p => p.MapOperationType == MapOperationType.Include).ForEach(part =>
            {
                var field = part as IFieldQueryMap;
                if (field == null)
                {
                    field = new FieldQueryPart(FieldHelper.TryExtractPropertyName(part.Expression), string.IsNullOrEmpty(entity.EntityAlias) ? entity.Entity : entity.EntityAlias, entity.Entity)
                    {
                        MapOperationType = MapOperationType.Include
                    };
                }

                queryParts.Add(field);
            });

            return entity;
        }

        #endregion

        #region ParameterQueryPart

        public static IParameterQueryPart CreateParameterQueryPart<T>(Expression<Func<T>> predicate)
        {
            return CreateParameterQueryPart(new IQueryMap[] { new QueryMap(MapOperationType.Value, predicate) });
        }

        //public static IParameterQueryPart CreateParameterQueryPart<T>(Expression<Func<IProcedureMapOption, IQueryMap>> part, Action<T> callback, ProcedureQueryPartsMap queryParts)
        //{
        //    return new CallbackParameterQueryPart<T>(new IQueryMap[] { QueryMapCompiler.Compile(part) }, callback)
        //    {
        //        MapOperationType = MapOperationType.Parameter
        //    };
        //}

        public static IParameterQueryPart CreateParameterQueryPart<T>(IQueryMap map, Action<T> callback, ProcedureQueryPartsMap queryParts)
        {
            return new CallbackParameterQueryPart<T>(new IQueryMap[] { map }, callback)
            {
                MapOperationType = MapOperationType.Parameter
            };
        }

        public static IParameterQueryPart CreateParameterQueryPart(IQueryMap[] querymap)
        {
            return new ParameterQueryPart(querymap)
            {
                MapOperationType = MapOperationType.Parameter
            };
        }

        #endregion

        //public static void AppendIncludes(IQueryPartsMap queryParts, IQueryMap[] parts)
        //{
        //    // set include
        //    parts.Where(p => p.MapOperationType == MapOperationType.Include).ForEach(part =>
        //    {
        //        //var field = part as FieldQueryPart;
        //        //if (field == null)
        //        //{
        //        var field = new FieldQueryPart(FieldHelper.ExtractPropertyName(part.Expression), string.IsNullOrEmpty(entity.EntityAlias) ? entity.Entity : entity.EntityAlias, entity.Entity)
        //        {
        //            MapOperationType = MapOperationType.Include
        //        };
        //        //}

        //        queryParts.Add(field);
        //    });
        //}
    }
}
