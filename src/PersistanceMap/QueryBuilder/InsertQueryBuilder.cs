using PersistanceMap.Internals;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace PersistanceMap.QueryBuilder
{
    public class InsertQueryBuilder<T> : IInsertQueryExpression<T>, IQueryExpression
    {
        public InsertQueryBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public InsertQueryBuilder(IDatabaseContext context, IQueryPartsMap container)
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

        /// <summary>
        /// Marks a propterty not to be included in the insert statement
        /// </summary>
        /// <param name="predicate">The property to ignore</param>
        /// <returns></returns>
        public IInsertQueryExpression<T> Ignore(Expression<Func<T, object>> predicate)
        {
            var insert = QueryPartsMap.Parts.OfType<IQueryPartDecorator>().FirstOrDefault(p => p.OperationType == OperationType.Insert);
            var value = QueryPartsMap.Parts.OfType<IQueryPartDecorator>().FirstOrDefault(p => p.OperationType == OperationType.Values);

            var fieldName = FieldHelper.TryExtractPropertyName(predicate);

            RemovePartByID(insert, fieldName);
            RemovePartByID(value, fieldName);

            return new InsertQueryBuilder<T>(Context, QueryPartsMap);
        }
        
        /// <summary>
        /// Inserts a row with the values defined in the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to insert</typeparam>
        /// <param name="dataPredicate">Expression providing the object containing the data</param>
        /// <returns></returns>
        public IInsertQueryExpression<T> Insert(Expression<Func<T>> dataPredicate)
        {
            return InsertInternal(dataPredicate);
        }

        /// <summary>
        /// Inserts a row with the values defined in the anonym dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to insert</typeparam>
        /// <param name="anonym">Expression providing the anonym object containing the data</param>
        /// <returns></returns>
        public IInsertQueryExpression<T> Insert(Expression<Func<object>> anonym)
        {
            return InsertInternal(anonym);
        }

        private IInsertQueryExpression<T> InsertInternal(LambdaExpression anonym)
        {
            var insertPart = new DelegateQueryPart(OperationType.Insert, () => string.Format("INSERT INTO {0} ", typeof(T).Name));
            QueryPartsMap.Add(insertPart);

            var valuesPart = new DelegateQueryPart(OperationType.Values, () => " VALUES ");
            QueryPartsMap.Add(valuesPart);

            var dataObject = anonym.Compile().DynamicInvoke();
            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>(dataObject.GetType());

            var first = tableFields.FirstOrDefault();
            var last = tableFields.LastOrDefault();

            foreach (var field in tableFields)
            {
                var value = field.GetValueFunction(dataObject);
                var quotated = DialectProvider.Instance.GetQuotedValue(value, field.MemberType);

                var fieldPart = new DelegateQueryPart(OperationType.None, () => string.Format("{0}{1}{2}", field == first ? "(" : "", field.MemberName, field == last ? ")" : ", "), field.MemberName);
                insertPart.Add(fieldPart);

                var valuePart = new DelegateQueryPart(OperationType.None, () => string.Format("{0}{1}{2}", field == first ? "(" : "", quotated, field == last ? ")" : ", "), field.MemberName);
                valuesPart.Add(valuePart);
            }

            return new InsertQueryBuilder<T>(Context, QueryPartsMap);
        }

        private static void RemovePartByID(IQueryPartDecorator decorator, string id)
        {
            if (decorator != null)
            {
                // remove the ignored element
                var subpart = decorator.Parts.FirstOrDefault(f => f.ID == id);
                if (subpart != null)
                    decorator.Remove(subpart);

                // make sure the first statement is correct.
                var first = decorator.Parts.FirstOrDefault();
                if (first != null)
                {
                    var value = first.Compile();
                    if (!value.StartsWith("("))
                    {
                        decorator.Remove(first);
                        decorator.Insert(0, new DelegateQueryPart(OperationType.None, () => string.Format("{0}{1}", "(", value), first.ID));
                    }
                }

                // make sure the last statement is correct
                var last = decorator.Parts.LastOrDefault();
                if (last != null)
                {
                    var value = last.Compile();
                    if (!value.TrimEnd().EndsWith(")"))
                    {
                        value = value.Replace(",", "").Replace(" ", "");

                        decorator.Remove(last);
                        decorator.Add(new DelegateQueryPart(OperationType.None, () => string.Format("{0}{1}", value, ")"), last.ID));
                    }
                }
            }
        }
    }
}
