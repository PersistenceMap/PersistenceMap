using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public class UpdateQueryBuilder<T> : IUpdateQueryExpression<T>, IQueryExpression
    {
        public UpdateQueryBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public UpdateQueryBuilder(IDatabaseContext context, IQueryPartsMap container)
        {
            _context = context;
            _queryPartsMap = container;
        }

     
        #region IQueryProvider Implementation

        readonly IDatabaseContext _context;
        public IDatabaseContext Context
        {
            get
            {
                return _context;
            }
        }

        IQueryPartsMap _queryPartsMap;
        public IQueryPartsMap QueryPartsMap
        {
            get
            {
                if (_queryPartsMap == null)
                    _queryPartsMap = new QueryPartsMap();
                return _queryPartsMap;
            }
        }

        #endregion

        //public IUpdateQueryExpression<T> AddToStore()
        //{
        //    Context.AddQuery(new UpdateQueryCommand(QueryPartsMap));

        //    return this;
        //}

        public IUpdateQueryExpression<T> Ignore(Expression<Func<T, object>> predicate)
        {
            var set = QueryPartsMap.Parts.FirstOrDefault(p => p.OperationType == OperationType.Set) as IQueryPartDecorator;

            var fieldName = FieldHelper.TryExtractPropertyName(predicate);

            RemovePartByID(set, fieldName);

            return new UpdateQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Updates a row with the values provided by the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to update</typeparam>
        /// <param name="dataPredicate">Expression providing the object containing the data</param>
        /// <param name="where">The expression providing the where statement</param>
        /// <returns></returns>
        public IUpdateQueryExpression<T> Update(Expression<Func<T>> dataPredicate, Expression<Func<T, object>> where = null)
        {
            // create expression containing key and value for the where statement
            var whereexpr = ExpressionFactory.CreateKeyExpression(dataPredicate, where);
            if (whereexpr == null)
            {
                // find the property called ID or {objectname}ID to define the where expression
                whereexpr = ExpressionFactory.CreateKeyExpression(dataPredicate);
            }

            QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.Update);
            var simple = QueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Set);

            var keyName = FieldHelper.TryExtractPropertyName(whereexpr);

            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>();
            var dataObject = dataPredicate.Compile().Invoke();
            var last = tableFields.Where(f => f.MemberName != keyName).LastOrDefault();
            foreach (var field in tableFields.Where(f => f.MemberName != keyName))
            {
                var value = DialectProvider.Instance.GetQuotedValue(field.GetValueFunction(dataObject), field.MemberType);
                var formatted = string.Format("{0} = {1}{2}", field.FieldName, value ?? "NULL", field.MemberName == last.MemberName ? " " : ", ");
                simple.Add(new DelegateQueryPart(OperationType.None, () => formatted, field.MemberName));
            }

            QueryPartsBuilder.Instance.AddExpressionQueryPart(QueryPartsMap, whereexpr, OperationType.Where);

            return new UpdateQueryBuilder<T>(Context, QueryPartsMap);
        }

        /// <summary>
        /// Updates a row with the values provided by the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to update</typeparam>
        /// <param name="anonym">Expression providing the anonym object containing the data</param>
        /// <param name="where">The expression providing the where statement</param>
        /// <returns></returns>
        public IUpdateQueryExpression<T> Update(Expression<Func<object>> anonym, Expression<Func<T, bool>> where = null)
        {
            // create expression containing key and value for the where statement
            var whereexpr = where;
            if (whereexpr == null)
            {
                // find the property called ID or {objectname}ID to define the where expression
                whereexpr = ExpressionFactory.CreateKeyExpression<T>(anonym);
            }

            QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.Update);
            var set = QueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Set);

            var keyName = FieldHelper.TryExtractPropertyName(whereexpr);

            var dataObject = anonym.Compile().Invoke();
            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>(dataObject.GetType());
            var last = tableFields.Where(f => f.MemberName != keyName).LastOrDefault();
            foreach (var field in tableFields.Where(f => f.MemberName != keyName))
            {
                var value = DialectProvider.Instance.GetQuotedValue(field.GetValueFunction(dataObject), field.MemberType);
                var formatted = string.Format("{0} = {1}{2}", field.FieldName, value ?? "NULL", field == last ? " " : ", ");
                set.Add(new DelegateQueryPart(OperationType.None, () => formatted, field.MemberName));
            }

            QueryPartsBuilder.Instance.AddExpressionQueryPart(QueryPartsMap, whereexpr, OperationType.Where);

            return new UpdateQueryBuilder<T>(Context, QueryPartsMap);
        }

        private static void RemovePartByID(IQueryPartDecorator decorator, string id)
        {
            //throw new NotImplementedException();
            if (decorator != null)
            {
                // remove the ignored element
                var subpart = decorator.Parts.FirstOrDefault(f => f.ID == id);
                if (subpart != null)
                    decorator.Remove(subpart);

                // make sure the last statement is correct
                var last = decorator.Parts.LastOrDefault();
                if (last != null)
                {
                    var value = last.Compile();
                    if (value.TrimEnd().EndsWith(","))
                    {
                        value = value.Replace(",", "").TrimEnd();//.Replace(" ", "");

                        decorator.Remove(last);
                        decorator.Add(new DelegateQueryPart(OperationType.None, () => string.Format("{0} ", value), last.ID));
                    }
                }
            }
        }
    }
}
