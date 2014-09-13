using System;
using System.Linq.Expressions;
using PersistanceMap.Internals;
using PersistanceMap.QueryBuilder.QueryPartsBuilders;
using PersistanceMap.QueryParts;

namespace PersistanceMap.QueryBuilder
{
    /// <summary>
    /// Containes logic to create queries that delete data from a database
    /// </summary>
    public class DeleteQueryBuilder : QueryPartsBuilder, IDeleteQueryProvider, IQueryProvider
    {
        public DeleteQueryBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public DeleteQueryBuilder(IDatabaseContext context, QueryPartsMap container)
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
                    _queryPartsMap = new SelectQueryPartsMap();
                return _queryPartsMap;
            }
        }

        #endregion


        public void Delete<T>()
        {
            var queryParts = new QueryPartsMap();

            AppendSimpleQueryPart(queryParts, OperationType.Delete);

            AppendEntityQueryPart<T>(queryParts, OperationType.From);

            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(queryParts);

            Context.Execute(query);
        }

        /// <summary>
        /// Deletes a record based on the where expression
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="where">The expression defining the where statement</param>
        public void Delete<T>(Expression<Func<T, bool>> where)
        {
            var queryParts = new QueryPartsMap();

            AppendSimpleQueryPart(queryParts, OperationType.Delete);

            AppendEntityQueryPart<T>(queryParts, OperationType.From);

            AppendExpressionQueryPart(queryParts, where, OperationType.Where);

            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(queryParts);

            Context.Execute(query);
        }

        /// <summary>
        /// Deletes a record based on the Properties and values of the given entity
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="entity">The entity to delete</param>
        /// <param name="key">The property defining the key on the entity</param>
        public void Delete<T>(Expression<Func<T>> entity, Expression<Func<T, object>> key = null)
        {
            // create expression containing key and value
            var whereexpr = ExpressionFactory.CreateKeyExpression(entity, key);
            if (whereexpr == null)
            {
                // find the property called ID or {objectname}ID to define the where expression
                whereexpr = ExpressionFactory.CreateKeyExpression(entity);
            }

            var queryParts = new QueryPartsMap();

            AppendSimpleQueryPart(queryParts, OperationType.Delete);

            AppendEntityQueryPart<T>(queryParts, OperationType.From);

            AppendExpressionQueryPart(queryParts, whereexpr, OperationType.Where);

            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(queryParts);

            Context.Execute(query);
        }

        public void Delete<T>(Expression<Func<object>> anonym)
        {
            // delete item that matches all properties set in the object
            throw new NotImplementedException();
        }
    }
}
