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

        public static EntityQueryPart<T> AppendEntityQueryPart<T>(IQueryPartsMap queryParts, Expression<Func<T, bool>> predicate, OperationType operation)
        {
            var expression = new ExpressionQueryPart(OperationType.On, predicate);

            return AppendEntityQueryPart<T>(queryParts, new IExpressionQueryPart[] { expression }, operation);
        }

        public static EntityQueryPart<T> AppendEntityQueryPart<T, T2>(IQueryPartsMap queryParts, Expression<Func<T, T2, bool>> predicate, OperationType operation)
        {
            var expression = new ExpressionQueryPart(OperationType.On, predicate);

            return AppendEntityQueryPart<T>(queryParts, new IExpressionQueryPart[] { expression }, operation);
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

        public static IExpressionQueryPart AppendExpressionQueryPart(IQueryPartsMap queryParts, LambdaExpression predicate, OperationType operation)
        {
            var part = new ExpressionQueryPart(operation, predicate);
            queryParts.Add(part);

            return part;
        }

        public static void AddFiedlParts(SelectQueryPartsMap queryParts, IFieldQueryMap[] fields)
        {
            foreach (var part in queryParts.Parts.Where(p => p.OperationType == OperationType.SelectMap))
            {
                var map = part as IQueryPartDecorator;
                if (map == null)
                    continue;

                foreach (var field in fields)
                {
                    if (map.Parts.Any(f => f is IFieldQueryMap && ((IFieldQueryMap)f).Field == field.Field || ((IFieldQueryMap)f).FieldAlias == field.Field))
                        continue;

                    map.Add(field);
                }
            }
        }

        #endregion

        #region ParameterQueryPart

        public static IParameterQueryPart AppendParameterQueryPart<T>(IQueryPartsMap queryParts, Expression<Func<T>> predicate, string name = null)
        {
            if (!string.IsNullOrEmpty(name))
            {
                // parameters have to start with @
                if (!name.StartsWith("@"))
                    name = string.Format("@{0}", name);
            }

            var part = new ParameterQueryPart(new IExpressionQueryPart[] { new ParameterQueryMap(OperationType.Value, name, predicate) })
            {
                OperationType = OperationType.Parameter
            };

            queryParts.Add(part);

            return part;
        }

        public static IParameterQueryPart AppendParameterQueryPart<T>(IQueryPartsMap queryParts, IExpressionQueryPart map, Action<T> callback)
        {
            var part = new CallbackParameterQueryPart<T>(new IExpressionQueryPart[] { map }, callback)
            {
                OperationType = OperationType.Parameter
            };

            queryParts.Add(part);

            return part;
        }

        #endregion
    }
}
