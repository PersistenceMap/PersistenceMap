using PersistanceMap.Compiler;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.Expressions
{
    public class SelectExpression<T> : ISelectExpression<T>
    {
        public SelectExpression(IDbContext context)
        {
            _context = context;
        }

        public SelectExpression(IDbContext context, SelectQueryPartsMap container)
        {
            _context = context;
            _queryPartsMap = container;
        }

        readonly IDbContext _context;
        public IDbContext Context
        {
            get
            {
                return _context;
            }
        }

        SelectQueryPartsMap _queryPartsMap;
        public SelectQueryPartsMap QueryPartsMap
        {
            get
            {
                if (_queryPartsMap == null)
                    _queryPartsMap = new SelectQueryPartsMap();
                return _queryPartsMap;
            }
        }

        IQueryPartsMap IPersistanceExpression.QueryPartsMap
        {
            get
            {
                return QueryPartsMap;
            }
        }

        #region Internal Implementation

        internal ISelectExpression<T2> From<T2>()
        {
            QueryPartsMap.Add(typeof(T2).ToFromQueryPart<T>());

            return new SelectExpression<T2>(Context, QueryPartsMap);
        }

        internal ISelectExpression<T2> From<T2>(params IMapQueryPart[] parts)
        {
            parts.EnsureArgumentNotNull("part");

            var fromPart = typeof(T2).ToFromQueryPart<T>();

            parts.ForEach(part =>
            {
                if (part.MapOperationType != MapOperationType.Identifier && part.MapOperationType != MapOperationType.Include)
                    throw new ArgumentException("Only IExpressionMapQueryPart of type Identifier or of type Iclude are allowed in a From expression", "parts");

                if (part.MapOperationType == MapOperationType.Identifier)
                    fromPart.Identifier = part.Expression.Compile().DynamicInvoke() as string;

                if (part.MapOperationType == MapOperationType.Include)
                {
                    fromPart.AddOperation(part);
                }
            });

            QueryPartsMap.Add(fromPart);

            return new SelectExpression<T2>(Context, QueryPartsMap);
        }

        #endregion

        #region ISqlExpression<T> Implementation

        public IEnumerable<T2> Select<T2>()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T2>(QueryPartsMap);

            return Context.Execute<T2>(query);
        }

        public IEnumerable<T> Select()
        {
            var expr = Context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(QueryPartsMap);

            return Context.Execute<T>(query);
        }




        public T2 Single<T2>()
        {
            throw new NotImplementedException();
        }





        public ISelectExpression<T> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate)
        {
            QueryPartsMap.Add(typeof(TJoin).ToJoinQueryPart<TJoin, T>(predicate));

            return new SelectExpression<T>(Context, QueryPartsMap);
        }

        public ISelectExpression<T> Join<TJoin>(params Expression<Func<SelectMapOption<TJoin, T>, IMapQueryPart>>[] args)
        {
            QueryPartsMap.Add(typeof(TJoin).ToJoinQueryPart<TJoin, T>(MapOptionCompiler.Compile<TJoin, T>(args)));

            return new SelectExpression<T>(Context, QueryPartsMap);
        }




        public ISelectExpression<T> Where(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ISelectExpression<T> Where<T2>(Expression<Func<T2, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ISelectExpression<T> Where<T2, T3>(params Expression<Func<SelectMapOption<T2, T3>, IMapQueryPart>>[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
