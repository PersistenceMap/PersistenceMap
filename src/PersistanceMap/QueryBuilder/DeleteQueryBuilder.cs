using System;
using System.Linq;
using System.Linq.Expressions;
using PersistanceMap.Factories;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;
using PersistanceMap.QueryParts;

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

            QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);

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

            QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);

            QueryPartsBuilder.Instance.AddExpressionQueryPart(QueryPartsMap, where, OperationType.Where);

            return new DeleteQueryBuilder(Context, QueryPartsMap);
        }

        /// <summary>
        /// Deletes a record based on the Properties and values of the given entity
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="dataObject">The entity to delete</param>
        /// <param name="where">The property defining the key on the entity</param>
        public IDeleteQueryExpression Delete<T>(Expression<Func<T>> dataObject, Expression<Func<T, object>> where = null)
        {
            // create expression containing key and value for the where statement
            var whereexpr = ExpressionFactory.CreateKeyExpression(dataObject, where);
            if (whereexpr == null)
            {
                // find the property called ID or {objectname}ID to define the where expression
                whereexpr = ExpressionFactory.CreateKeyExpression(dataObject);
            }

            var deletePart = new DelegateQueryPart(OperationType.Delete, () => "DELETE ");
            QueryPartsMap.Add(deletePart);

            QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);

            QueryPartsBuilder.Instance.AddExpressionQueryPart(QueryPartsMap, whereexpr, OperationType.Where);

            return new DeleteQueryBuilder(Context, QueryPartsMap);
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

            QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);

            // create expressions of all properties and theyr values
            var expressions = ExpressionFactory.CreateKeyValueEqualityExpressions<T>(obj, anonymFields, tableFields).ToList();

            var first = expressions.FirstOrDefault();
            foreach (var expr in expressions)
            {
                // add all expressions to the queryexpression
                QueryPartsBuilder.Instance.AddExpressionQueryPart(QueryPartsMap, expr, first == expr ? OperationType.Where : OperationType.And);
            }

            return new DeleteQueryBuilder(Context, QueryPartsMap);
        }
    }
}
