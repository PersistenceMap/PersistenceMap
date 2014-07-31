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

        public static IQueryPart AppendSelectMapQueryPart(IQueryPartsMap queryParts, OperationType operation)
        {
            var part = new SelectMapQueryPart(operation);

            queryParts.Add(part);

            return part;
        }

        public static IQueryPart AppendQueryPart(IQueryPartsMap queryParts, OperationType operation, Func<string> predicate)
        {
            var part = new PredicateQueryPart(operation, predicate);

            queryParts.Add(part);

            return part;
        }

        public static EntityQueryPart<T> AppendEntityQueryPart<T>(IQueryPartsMap queryParts, OperationType operation)
        {
            var type = typeof(T);
            var entity = new EntityQueryPart<T>(type.Name)
            {
                OperationType = operation
            };

            queryParts.Add(entity);

            return entity;
        }

        public static EntityQueryPart<T> AppendEntityQueryPart<T, T2>(IQueryPartsMap queryParts, Expression<Func<T, T2, bool>> predicate, OperationType maptype)
        {
            var operation = new ExpressionQueryPart(OperationType.On, predicate);

            return AppendEntityQueryPart<T>(queryParts, new IExpressionQueryPart[] { operation }, maptype);
        }

        public static EntityQueryPart<T> AppendEntityQueryPart<T>(IQueryPartsMap queryParts, IExpressionQueryPart[] parts, OperationType maptype)
        {
            var operationParts = parts.Where(p => p.OperationType == OperationType.On || p.OperationType == OperationType.And || p.OperationType == OperationType.Or).ToArray();

            var type = typeof(T);
            var entity = new EntityQueryPart<T>(type.Name, null, operationParts)
            {
                OperationType = maptype
            };

            queryParts.Add(entity);

            // first set alias
            //var id = parts.Where(p => p.OperationType == OperationType.As).Reverse().FirstOrDefault();
            //if (id != null)
            //{
            //    throw new NotImplementedException("Test if this is still needed!");

            //    entity.EntityAlias = id.Expression.Compile().DynamicInvoke() as string;
            //    if (!string.IsNullOrEmpty(entity.EntityAlias))
            //    {
            //        foreach (var part in entity.Parts)
            //        {
            //            var map = part as IExpressionQueryPart;
            //            if (map == null)
            //                continue;

            //            map.AliasMap.Add(typeof(T), entity.EntityAlias);
            //        }
            //    }
            //}

            //// set include
            //parts.Where(p => p.OperationType == OperationType.Include).ForEach(part =>
            //{
            //    throw new NotImplementedException("Test if this is still needed!");

            //    var field = part as IFieldQueryMap;
            //    if (field == null)
            //    {
            //        field = new FieldQueryPart(FieldHelper.TryExtractPropertyName(part.Expression), string.IsNullOrEmpty(entity.EntityAlias) ? entity.Entity : entity.EntityAlias, entity.Entity)
            //        {
            //            OperationType = OperationType.Include
            //        };
            //    }

            //    queryParts.Add(field);
            //});

            return entity;
        }


        public static IFieldQueryMap AppendFieldQueryMap(IQueryPartsMap queryParts, string field, string alias, string entity, string entityalias)
        {
            var part = new FieldQueryPart(field, alias, null /*EntityAlias*/, entity)
            {
                OperationType = OperationType.Include
            };

            queryParts.Add(part);

            return part;
        }

        #endregion

        #region ParameterQueryPart

        public static IParameterQueryPart CreateParameterQueryPart<T>(Expression<Func<T>> predicate)
        {
            return CreateParameterQueryPart(new IExpressionQueryPart[] { new ExpressionQueryPart(OperationType.Value, predicate) });
        }

        //public static IParameterQueryPart CreateParameterQueryPart<T>(Expression<Func<IProcedureMapOption, IQueryMap>> part, Action<T> callback, ProcedureQueryPartsMap queryParts)
        //{
        //    return new CallbackParameterQueryPart<T>(new IQueryMap[] { QueryMapCompiler.Compile(part) }, callback)
        //    {
        //        OperationType = OperationType.Parameter
        //    };
        //}

        public static IParameterQueryPart CreateParameterQueryPart<T>(IExpressionQueryPart map, Action<T> callback, ProcedureQueryPartsMap queryParts)
        {
            return new CallbackParameterQueryPart<T>(new IExpressionQueryPart[] { map }, callback)
            {
                OperationType = OperationType.Parameter
            };
        }

        public static IParameterQueryPart CreateParameterQueryPart(IExpressionQueryPart[] querymap)
        {
            return new ParameterQueryPart(querymap)
            {
                OperationType = OperationType.Parameter
            };
        }

        #endregion
    }
}
