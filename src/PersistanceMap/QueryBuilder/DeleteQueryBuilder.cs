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
                    _queryPartsMap = new SelectQueryPartsMap();
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
            AppendSimpleQueryPart(QueryPartsMap, OperationType.Delete);

            AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);

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
            AppendSimpleQueryPart(QueryPartsMap, OperationType.Delete);

            AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);

            AppendExpressionQueryPart(QueryPartsMap, where, OperationType.Where);

            //var expr = Context.ContextProvider.ExpressionCompiler;
            //var query = expr.Compile<T>(queryParts);

            //Context.Execute(query);
            return new DeleteQueryBuilder(Context, QueryPartsMap);
        }

        /// <summary>
        /// Deletes a record based on the Properties and values of the given entity
        /// </summary>
        /// <typeparam name="T">The Type that defines the Table to delete from</typeparam>
        /// <param name="entity">The entity to delete</param>
        /// <param name="key">The property defining the key on the entity</param>
        public IDeleteQueryProvider Delete<T>(Expression<Func<T>> entity, Expression<Func<T, object>> key = null)
        {
            // create expression containing key and value
            var whereexpr = ExpressionFactory.CreateKeyExpression(entity, key);
            if (whereexpr == null)
            {
                // find the property called ID or {objectname}ID to define the where expression
                whereexpr = ExpressionFactory.CreateKeyExpression(entity);
            }

            AppendSimpleQueryPart(QueryPartsMap, OperationType.Delete);

            AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);

            AppendExpressionQueryPart(QueryPartsMap, whereexpr, OperationType.Where);

            //var expr = Context.ContextProvider.ExpressionCompiler;
            //var query = expr.Compile<T>(queryParts);

            //Context.Execute(query);
            return new DeleteQueryBuilder(Context, QueryPartsMap);
        }

        public IDeleteQueryProvider Delete<T>(Expression<Func<object>> anonym)
        {
            // delete item that matches all properties set in the object
            throw new NotImplementedException();
        }
    }
}
