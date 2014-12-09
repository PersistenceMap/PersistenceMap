using System;
using System.Linq;
using System.Linq.Expressions;
using PersistanceMap.Factories;
using PersistanceMap.QueryParts;
using PersistanceMap.Sql;
using PersistanceMap.Tracing;

namespace PersistanceMap.QueryBuilder
{
    /// <summary>
    /// Containes logic to create queries that delete data from a database
    /// </summary>
    public class DeleteQueryBuilder : IDeleteQueryExpression, IQueryExpression
    {
        public DeleteQueryBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public DeleteQueryBuilder(IDatabaseContext context, IQueryPartsMap container)
        {
            _context = context;
            _queryPartsMap = container;
        }

        private ILogger _logger;
        protected ILogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = Context.LoggerFactory.CreateLogger();
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
        
        public IDeleteQueryExpression Delete<T>()
        {
            var deletePart = new DelegateQueryPart(OperationType.Delete, () => "DELETE ");
            QueryPartsMap.Add(deletePart);

            var entityPart = new DelegateQueryPart(OperationType.From, () => string.Format("FROM {0} ", typeof(T).Name));
            QueryPartsMap.Add(entityPart);

            return new DeleteQueryBuilder(Context, QueryPartsMap);
        }

        /// <summary>
        /// Deletes a record based on the where expression
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="where">The expression defining the where statement</param>
        public IDeleteQueryExpression Delete<T>(Expression<Func<T, bool>> where)
        {
            var deletePart = new DelegateQueryPart(OperationType.Delete, () => "DELETE ");
            QueryPartsMap.Add(deletePart);

            var entityPart = new DelegateQueryPart(OperationType.From, () => string.Format("FROM {0} ", typeof(T).Name));
            QueryPartsMap.Add(entityPart);

            var part = new DelegateQueryPart(OperationType.Where, () => string.Format("WHERE {0} ", LambdaToSqlCompiler.Compile(where)));
            QueryPartsMap.Add(part);

            return new DeleteQueryBuilder(Context, QueryPartsMap);
        }

        /// <summary>
        /// Deletes a record based on the Properties and values of the given entity
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="dataObject">The entity to delete</param>
        /// <param name="identification">The property defining the identification/key property on the entity</param>
        public IDeleteQueryExpression Delete<T>(Expression<Func<T>> dataObject, Expression<Func<T, object>> identification = null)
        {
            try
            {
                // create expression containing key and value for the where statement
                var whereexpr = ExpressionFactory.CreateKeyExpression(dataObject, identification);
                if (whereexpr == null)
                {
                    // find the property called ID or {objectname}ID to define the where expression
                    whereexpr = ExpressionFactory.CreateKeyExpression(dataObject);
                }

                var deletePart = new DelegateQueryPart(OperationType.Delete, () => "DELETE ");
                QueryPartsMap.Add(deletePart);

                var entityPart = new DelegateQueryPart(OperationType.From, () => string.Format("FROM {0} ", typeof (T).Name));
                QueryPartsMap.Add(entityPart);

                var part = new DelegateQueryPart(OperationType.Where, () => string.Format("WHERE {0} ", LambdaToSqlCompiler.Compile(whereexpr)));
                QueryPartsMap.Add(part);

                return new DeleteQueryBuilder(Context, QueryPartsMap);
            }
            catch (Exception e)
            {
                Logger.Write(e.Message, category: LoggerCategory.Error, logtime: DateTime.Now);
                throw;
            }
        }

        /// <summary>
        /// Delete a record based on the Properties and values passed in the anonym object
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="anonym">The object that defines the properties and the values that mark the object to delete</param>
        /// <returns>IDeleteQueryProvider</returns>
        public IDeleteQueryExpression Delete<T>(Expression<Func<object>> anonym)
        {
            // delete item that matches all properties set in the object
            var obj = anonym.Compile().Invoke();

            var anonymFields = TypeDefinitionFactory.GetFieldDefinitions(obj);
            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>();

            var deletePart = new DelegateQueryPart(OperationType.Delete, () => "DELETE ");
            QueryPartsMap.Add(deletePart);

            var entityPart = new DelegateQueryPart(OperationType.From, () => string.Format("FROM {0} ", typeof(T).Name));
            QueryPartsMap.Add(entityPart);

            // create expressions of all properties and theyr values
            var expressions = ExpressionFactory.CreateKeyValueEqualityExpressions<T>(obj, anonymFields, tableFields).ToList();

            var first = expressions.FirstOrDefault();
            foreach (var expr in expressions)
            {
                // add all expressions to the queryexpression
                var part = new DelegateQueryPart(first == expr ? OperationType.Where : OperationType.And, () => string.Format("{0} {1} ", first == expr ? "WHERE" : "AND", LambdaToSqlCompiler.Compile(expr)));
                QueryPartsMap.Add(part);
            }

            return new DeleteQueryBuilder(Context, QueryPartsMap);
        }
    }
}
