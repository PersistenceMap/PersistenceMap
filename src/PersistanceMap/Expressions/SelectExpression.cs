using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PersistanceMap.Expressions
{
    public class SelectExpression<T> : ISelectExpression<T>
    {
        IDbContext _context;

        QueryPartsContainer _queryPartContainer;
        public QueryPartsContainer QueryPartContainer
        {
            get
            {
                if (_queryPartContainer == null)
                    _queryPartContainer = new QueryPartsContainer();
                return _queryPartContainer;
            }
        }

        public SelectExpression(IDbContext context)
        {
            _context = context;
        }

        public SelectExpression(IDbContext context, QueryPartsContainer container)
        {
            _context = context;
            _queryPartContainer = container;
        }

        #region Internal Implementation

        internal ISelectExpression<T> From<T>()
        {
            QueryPartContainer.Add(typeof(T).ToFromQueryPart<T>());

            return new SelectExpression<T>(_context, QueryPartContainer);
        }

        internal ISelectExpression<T> From<T>(params IExpressionMapQueryPart[] parts)
        {
            parts.EnsureArgumentNotNull("part");

            var fromPart = typeof(T).ToFromQueryPart<T>();

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

            QueryPartContainer.Add(fromPart);

            return new SelectExpression<T>(_context, QueryPartContainer);
        }

        #endregion

        #region ISqlExpression<T> Implementation

        public IEnumerable<T> Select<T>()
        {
            var expr = _context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(QueryPartContainer);

            return _context.Execute<T>(query);
        }

        public IEnumerable<T> Select()
        {
            var expr = _context.ContextProvider.ExpressionCompiler;
            var query = expr.Compile<T>(QueryPartContainer);

            return _context.Execute<T>(query);
        }




        public T Single<T>()
        {
            throw new NotImplementedException();
        }





        public ISelectExpression<T> Join<TJoin>(Expression<Func<TJoin, T, bool>> predicate)
        {
            QueryPartContainer.Add(typeof(TJoin).ToJoinQueryPart<TJoin, T>(predicate));

            return new SelectExpression<T>(_context, QueryPartContainer);
        }

        public ISelectExpression<T> Join<TJoin>(params Expression<Func<MapOption<TJoin, T>, IExpressionMapQueryPart>>[] args)
        {

            QueryPartContainer.Add(typeof(TJoin).ToJoinQueryPart<TJoin, T>(MapOptionCompiler.Compile<TJoin, T>(args)));

            return new SelectExpression<T>(_context, QueryPartContainer);
        }




        public ISelectExpression<T> Where(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ISelectExpression<T> Where<T2>(Expression<Func<T2, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ISelectExpression<T> Where<T2, T3>(params Expression<Func<MapOption<T2, T3>, IExpressionMapQueryPart>>[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
