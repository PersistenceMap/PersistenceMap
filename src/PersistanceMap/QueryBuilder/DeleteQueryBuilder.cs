using System;
using System.Linq;
using System.Linq.Expressions;
using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;
using PersistanceMap.QueryBuilder.Commands;
using PersistanceMap.QueryParts;

namespace PersistanceMap.QueryBuilder
{
    /// <summary>
    /// Containes logic to create queries that delete data from a database
    /// </summary>
    public class DeleteQueryBuilder : IDeleteQueryProvider, IQueryProvider
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

        public IDeleteQueryProvider AddToStore()
        {
            Context.AddQuery(new DeleteQueryCommand(QueryPartsMap));

            return this;
        }

        public IDeleteQueryProvider Delete<T>()
        {
            QueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Delete);

            QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);

            //var expr = Context.ContextProvider.ExpressionCompiler;
            //var query = expr.Compile<T>(queryParts);

            //Context.Execute(query);
            return new DeleteQueryBuilder(Context, QueryPartsMap);
        }

        /// <summary>
        /// Deletes a record based on the where expression
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="where">The expression defining the where statement</param>
        public IDeleteQueryProvider Delete<T>(Expression<Func<T, bool>> where)
        {
            QueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Delete);

            QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);

            QueryPartsBuilder.Instance.AppendExpressionQueryPart(QueryPartsMap, where, OperationType.Where);

            //var expr = Context.ContextProvider.ExpressionCompiler;
            //var query = expr.Compile<T>(queryParts);

            //Context.Execute(query);
            return new DeleteQueryBuilder(Context, QueryPartsMap);
        }

        /// <summary>
        /// Deletes a record based on the Properties and values of the given entity
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="dataObject">The entity to delete</param>
        /// <param name="where">The property defining the key on the entity</param>
        public IDeleteQueryProvider Delete<T>(Expression<Func<T>> dataObject, Expression<Func<T, object>> where = null)
        {
            // create expression containing key and value for the where statement
            var whereexpr = ExpressionFactory.CreateKeyExpression(dataObject, where);
            if (whereexpr == null)
            {
                // find the property called ID or {objectname}ID to define the where expression
                whereexpr = ExpressionFactory.CreateKeyExpression(dataObject);
            }

            QueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Delete);

            QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);

            QueryPartsBuilder.Instance.AppendExpressionQueryPart(QueryPartsMap, whereexpr, OperationType.Where);

            //var expr = Context.ContextProvider.ExpressionCompiler;
            //var query = expr.Compile<T>(queryParts);

            //Context.Execute(query);
            return new DeleteQueryBuilder(Context, QueryPartsMap);
        }

        /// <summary>
        /// Delete a record based on the Properties and values passed in the anonym object
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="anonym">The object that defines the properties and the values that mark the object to delete</param>
        /// <returns>IDeleteQueryProvider</returns>
        public IDeleteQueryProvider Delete<T>(Expression<Func<object>> anonym)
        {
            // delete item that matches all properties set in the object
            var obj = anonym.Compile().Invoke();
            var anonymFields = TypeDefinitionFactory.GetFieldDefinitions(obj.GetType());
            var tableFields = TypeDefinitionFactory.GetFieldDefinitions<T>();

            QueryPartsBuilder.Instance.AppendSimpleQueryPart(QueryPartsMap, OperationType.Delete);

            QueryPartsBuilder.Instance.AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);

            // create expressions of all properties and theyr values
            var expressions = ExpressionFactory.CreateKeyValueEqualityExpressions<T>(obj, anonymFields, tableFields).ToList();

            var first = expressions.FirstOrDefault();
            foreach (var expr in expressions)
            {
                // add all expressions to the queryexpression
                QueryPartsBuilder.Instance.AppendExpressionQueryPart(QueryPartsMap, expr, first == expr ? OperationType.Where : OperationType.And);
            }

            return new DeleteQueryBuilder(Context, QueryPartsMap);
        }
    }
}
