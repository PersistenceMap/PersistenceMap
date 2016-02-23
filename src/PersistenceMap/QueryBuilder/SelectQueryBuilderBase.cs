using System;
using System.Linq.Expressions;
using PersistenceMap.QueryParts;
using PersistenceMap.Sql;
using PersistenceMap.Diagnostics;
using PersistenceMap.Ensure;
using System.Collections.Generic;
using System.Linq;

namespace PersistenceMap.QueryBuilder
{
    public class SelectQueryBuilderBase<T> : ISelectQueryExpressionBase<T>, IQueryExpression
    {
        public SelectQueryBuilderBase(IDatabaseContext context)
        {
            _context = context;
        }

        public SelectQueryBuilderBase(IDatabaseContext context, SelectQueryPartsContainer container)
        {
            _context = context;
            _queryParts = container;
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

        SelectQueryPartsContainer _queryParts;
        public SelectQueryPartsContainer QueryParts
        {
            get
            {
                if (_queryParts == null)
                {
                    _queryParts = new SelectQueryPartsContainer();
                }

                return _queryParts;
            }
        }

        IQueryPartsContainer IQueryExpression.QueryParts
        {
            get
            {
                return QueryParts;
            }
        }

        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type
        /// </summary>
        /// <typeparam name="T2">The type to return</typeparam>
        /// <returns></returns>
        public IEnumerable<T2> Select<T2>()
        {
            var query = Compile<T2>();

            return Context.Kernel.Execute<T2>(query);
        }

        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Select()
        {
            return Select<T>();
        }

        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type
        /// </summary>
        /// <typeparam name="TSelect">The type to return</typeparam>
        /// <param name="anonym">The type to return</param>
        /// <returns></returns>
        public IEnumerable<TSelect> Select<TSelect>(Expression<Func<TSelect>> anonym)
        {
            return Select<TSelect>();
        }

        /// <summary>
        /// Executes a select expression and maps the returnvalue to objects of the defined type and executes all objects to the delegate
        /// </summary>
        /// <typeparam name="TSelect">The type to return</typeparam>
        /// <param name="anonym">The delegate that gets executed for each returned object</param>
        /// <returns></returns>
        public IEnumerable<TSelect> Select<TSelect>(Expression<Func<T, TSelect>> anonym)
        {
            var elements = Select<T>();
            var expression = anonym.Compile();

            foreach (var item in elements)
            {
                yield return expression.Invoke(item);
            }
        }
        
        /// <summary>
        /// Defines the fields that will be used in the query
        /// </summary>
        /// <typeparam name="TNew"></typeparam>
        /// <returns></returns>
        public IAfterMapQueryExpression<TNew> For<TNew>()
        {
            var members = typeof(TNew).GetSelectionMembers();
            var fields = members.Select(m => m.ToFieldQueryPart(null, null));

            FieldQueryPart.FiedlPartsFactory(QueryParts, fields.ToArray());

            foreach (var part in QueryParts.Parts.Where(p => p.OperationType == OperationType.Select))
            {
                // seal part to disalow other parts to be added to selectmaps
                part.IsSealed = true;
            }

            return new AfterMapQueryBuilder<TNew>(Context, QueryParts);
        }

        /// <summary>
        /// Defines the fields that will be used in the query
        /// </summary>
        /// <typeparam name="TAno"></typeparam>
        /// <param name="anonym"></param>
        /// <returns></returns>
        public IAfterMapQueryExpression<TAno> For<TAno>(Expression<Func<TAno>> anonym)
        {
            return For<TAno>();
        }
        
        /// <summary>
        /// Compiles the Query to a sql statement for the given type
        /// </summary>
        /// <typeparam name="T2">The select type</typeparam>
        /// <returns>The sql string</returns>
        public string CompileQuery<T2>()
        {
            var query = Compile<T2>();

            return query.QueryString;
        }

        /// <summary>
        /// Compiles the Query to a sql statement
        /// </summary>
        /// <returns>The sql string</returns>
        public string CompileQuery()
        {
            return CompileQuery<T>();
        }

        private CompiledQuery Compile<T2>()
        {
            // get all members on the type to be composed
            var members = typeof(T2).GetSelectionMembers();

            // don't set entity alias to prevent fields being set with a default alias of the from expression
            var fields = members.Select(m => m.ToFieldQueryPart(null, null));
            FieldQueryPart.FiedlPartsFactory(QueryParts, fields.ToArray());

            var expr = Context.ConnectionProvider.QueryCompiler;
            var query = expr.Compile(QueryParts, Context.Interceptors);

            return query;
        }

        #endregion
    }
}
