using PersistanceMap.Factories;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using System;
using System.Linq;
using System.Linq.Expressions;
using PersistanceMap.Tracing;

namespace PersistanceMap.QueryBuilder
{
    public class UpdateQueryBuilder<T> : IUpdateQueryExpression<T>, IQueryExpression
    {
        public UpdateQueryBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public UpdateQueryBuilder(IDatabaseContext context, IQueryPartsContainer container)
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
        /// Marks a property not to be included in the update statement
        /// </summary>
        /// <param name="predicate">The property to ignore</param>
        /// <returns></returns>
        public IUpdateQueryExpression<T> Ignore(Expression<Func<T, object>> predicate)
        {
            var set = QueryParts.Parts.OfType<IQueryPartDecorator>().FirstOrDefault(p => p.OperationType == OperationType.Update) as IQueryPartDecorator;

            var fieldName = FieldHelper.TryExtractPropertyName(predicate);

            RemovePartByID(set, fieldName);

            return new UpdateQueryBuilder<T>(Context, QueryParts);
        }

        /// <summary>
        /// Updates a row with the values provided by the dataobject
        /// </summary>
        /// <typeparam name="T">Tabletype to update</typeparam>
        /// <param name="dataPredicate">Expression providing the object containing the data</param>
        /// <param name="identification">The expression providing the identification/key property  on the entity</param>
        /// <returns></returns>
        public IUpdateQueryExpression<T> Update(Expression<Func<T>> dataPredicate, Expression<Func<T, object>> identification = null)
        {
            // create expression containing key and value for the where statement
            var whereexpr = ExpressionFactory.CreateEqualityExpression(dataPredicate, identification);
            if (whereexpr == null)
            {
                // find the property called ID or {objectname}ID to define the where expression
                whereexpr = ExpressionFactory.CreateEqualityExpression(dataPredicate);
            }

            var updatePart = new DelegateQueryPart(OperationType.Update, () => typeof(T).Name);
            QueryParts.Add(updatePart);

            var keyName = FieldHelper.TryExtractPropertyName(whereexpr);

            var dataObject = dataPredicate.Compile().Invoke();

            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>();

            var last = tableFields.LastOrDefault(f => f.MemberName != keyName);

            foreach (var field in tableFields.Where(f => f.MemberName != keyName))
            {
                var value = DialectProvider.Instance.GetQuotedValue(field.GetValueFunction(dataObject), field.MemberType);
                var formatted = string.Format("{0} = {1}", field.FieldName, value ?? "NULL");

                var keyValuePart = new DelegateQueryPart(OperationType.UpdateValue, () => formatted, field.MemberName);
                updatePart.Add(keyValuePart);
            }

            var part = new DelegateQueryPart(OperationType.Where, () => LambdaToSqlCompiler.Compile(whereexpr).ToString());
            QueryParts.Add(part);

            return new UpdateQueryBuilder<T>(Context, QueryParts);
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
                whereexpr = ExpressionFactory.CreateEqualityExpression<T>(anonym);
            }

            var entity = new DelegateQueryPart(OperationType.Update, () => typeof(T).Name);
            QueryParts.Add(entity);
            
            var keyName = FieldHelper.TryExtractPropertyName(whereexpr);

            var dataObject = anonym.Compile().Invoke();

            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>(dataObject.GetType());

            var last = tableFields.LastOrDefault(f => f.MemberName != keyName);

            foreach (var field in tableFields.Where(f => f.MemberName != keyName))
            {
                var value = DialectProvider.Instance.GetQuotedValue(field.GetValueFunction(dataObject), field.MemberType);
                var formatted = string.Format("{0} = {1}", field.FieldName, value ?? "NULL");

                var keyValuePart = new DelegateQueryPart(OperationType.UpdateValue, () => formatted, field.MemberName);
                entity.Add(keyValuePart);
            }

            var part = new DelegateQueryPart(OperationType.Where, () => LambdaToSqlCompiler.Compile(whereexpr).ToString());
            QueryParts.Add(part);

            return new UpdateQueryBuilder<T>(Context, QueryParts);
        }

        private static void RemovePartByID(IQueryPartDecorator decorator, string id)
        {
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
                        value = value.Replace(",", "").TrimEnd();

                        decorator.Remove(last);
                        decorator.Add(new DelegateQueryPart(OperationType.None, () => string.Format("{0} ", value), last.ID));
                    }
                }
            }
        }
    }
}
