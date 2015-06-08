using PersistanceMap.Factories;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using System;
using System.Linq;
using System.Linq.Expressions;
using PersistanceMap.Tracing;

namespace PersistanceMap.QueryBuilder
{
    public class InsertQueryBuilder<T> : IInsertQueryExpression<T>, IQueryExpression
    {
        public InsertQueryBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public InsertQueryBuilder(IDatabaseContext context, IQueryPartsContainer container)
        {
            _context = context;
            _queryParts = container;
        }

        private ILogger _logger;
        protected ILogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = Context.Kernel.LoggerFactory.CreateLogger();
                return _logger;
            }
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

        IQueryPartsContainer _queryParts;
        public IQueryPartsContainer QueryParts
        {
            get
            {
                if (_queryParts == null)
                    _queryParts = new QueryPartsContainer();
                return _queryParts;
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
            var insert = QueryParts.Parts.OfType<IItemsQueryPart>().FirstOrDefault(p => p.OperationType == OperationType.Insert);
            var value = QueryParts.Parts.OfType<IItemsQueryPart>().FirstOrDefault(p => p.OperationType == OperationType.Values);

            var fieldName = FieldHelper.TryExtractPropertyName(predicate);

            RemovePartByID(insert, fieldName);
            RemovePartByID(value, fieldName);

            return new InsertQueryBuilder<T>(Context, QueryParts);
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
            var insertPart = new DelegateQueryPart(OperationType.Insert, () => typeof(T).Name);
            QueryParts.Add(insertPart);

            var valuesPart = new ItemsQueryPart(OperationType.Values);
            QueryParts.Add(valuesPart);

            var dataObject = anonym.Compile().DynamicInvoke();
            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>(dataObject.GetType());

            var first = tableFields.FirstOrDefault();
            var last = tableFields.LastOrDefault();

            foreach (var field in tableFields)
            {
                var value = field.GetValueFunction(dataObject);
                var quotated = DialectProvider.Instance.GetQuotedValue(value, field.MemberType);

                var fieldPart = new DelegateQueryPart(OperationType.InsertMember, () => field.MemberName, field.MemberName);
                insertPart.Add(fieldPart);

                var valuePart = new DelegateQueryPart(OperationType.InsertValue, () => quotated, field.MemberName);
                valuesPart.Add(valuePart);
            }

            return new InsertQueryBuilder<T>(Context, QueryParts);
        }

        private static void RemovePartByID(IItemsQueryPart decorator, string id)
        {
            if (decorator != null)
            {
                // remove the ignored element
                var subpart = decorator.Parts.FirstOrDefault(f => f.ID == id);
                if (subpart != null)
                    decorator.Remove(subpart);
            }
        }
    }
}
