using PersistanceMap.QueryParts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder.QueryPartsBuilders
{
    internal class QueryPartsBuilder
    {
        protected QueryPartsBuilder()
        {
        }

        private static QueryPartsBuilder instance;

        /// <summary>
        /// Gets the Singleton instance of the QueryPartsBuilder
        /// </summary>
        public static QueryPartsBuilder Instance
        {
            get
            {
                if (instance == null)
                    instance = new QueryPartsBuilder();

                return instance;
            }
        }

        internal SimpleQueryPart AppendSimpleQueryPart(IQueryPartsMap queryParts, OperationType operation)
        {
            var part = new SimpleQueryPart(operation);

            queryParts.Add(part);

            return part;
        }

        internal IQueryPart AppendQueryPart(IQueryPartsMap queryParts, OperationType operation, Func<string> predicate)
        {
            var part = new PredicateQueryPart(operation, predicate);

            queryParts.Add(part);

            return part;
        }

        internal EntityQueryPart AppendEntityQueryPart<T>(IQueryPartsMap queryParts, OperationType operation)
        {
            var type = typeof(T);
            var entity = new EntityQueryPart(type.Name)
            {
                OperationType = operation
            };

            queryParts.Add(entity);

            return entity;
        }

        internal EntityQueryPart AppendEntityQueryPart<T>(IQueryPartsMap queryParts, Expression<Func<T, bool>> predicate, OperationType operation)
        {
            var expression = new ExpressionQueryPart(OperationType.On, predicate);

            return AppendEntityQueryPart<T>(queryParts, new IExpressionQueryPart[] { expression }, operation);
        }

        internal EntityQueryPart AppendEntityQueryPart<T, T2>(IQueryPartsMap queryParts, Expression<Func<T, T2, bool>> predicate, OperationType operation)
        {
            var expression = new ExpressionQueryPart(OperationType.On, predicate);

            return AppendEntityQueryPart<T>(queryParts, new IExpressionQueryPart[] { expression }, operation);
        }

        internal EntityQueryPart AppendEntityQueryPart<T>(IQueryPartsMap queryParts, IExpressionQueryPart[] parts, OperationType maptype)
        {
            var operationParts = parts.Where(p => p.OperationType == OperationType.On || p.OperationType == OperationType.And || p.OperationType == OperationType.Or).ToArray();

            var type = typeof(T);
            var entity = new EntityQueryPart(type.Name, null, operationParts)
            {
                OperationType = maptype
            };

            queryParts.Add(entity);

            return entity;
        }

        internal IExpressionQueryPart AddExpressionQueryPart(IQueryPartsMap queryParts, LambdaExpression predicate, OperationType operation)
        {
            var part = new ExpressionQueryPart(operation, predicate);
            queryParts.Add(part);

            return part;
        }

        internal IFieldQueryPart AddFieldQueryMap(IQueryPartsMap queryParts, string field, string alias, string entity, string entityalias)
        {
            var part = new FieldQueryPart(field, alias, null /*EntityAlias*/, entity)
            {
                OperationType = OperationType.Include
            };

            queryParts.Add(part);

            return part;
        }

        internal void AddFiedlParts(SelectQueryPartsMap queryParts, FieldQueryPart[] fields)
        {
            foreach (var part in queryParts.Parts.Where(p => p.OperationType == OperationType.Select))
            {
                var map = part as IQueryPartDecorator;
                if (map == null)
                    continue;

                var mappedFields = map.Parts.ToList();

                //var last = fields.LastOrDefault();
                foreach (var field in fields)
                {
                    //if (map.Parts.Any(f => f is FieldQueryPart && ((FieldQueryPart)f).Field == field.Field || ((FieldQueryPart)f).FieldAlias == field.Field))
                    //    continue;

                    //var sufix = ", ";
                    //if (field == last)
                    //    sufix = " ";

                    var mappedField = map.Parts.FirstOrDefault(f => f is FieldQueryPart && ((FieldQueryPart)f).Field == field.Field || ((FieldQueryPart)f).FieldAlias == field.Field);
                    if (mappedField != null)
                    {
                        ((FieldQueryPart)mappedField).Sufix = ", ";
                        mappedFields.Remove(mappedField);
                        continue;
                    }

                    field.Sufix = ", ";

                    map.Add(field);
                }

                // remove all mapped fields that were not included in the select fields
                foreach (var field in mappedFields)
                {
                    map.Remove(field);
                }

                var last = map.Parts.LastOrDefault(p => p is FieldQueryPart) as FieldQueryPart;
                if (last != null)
                    last.Sufix = " ";
            }
        }
    }
}
