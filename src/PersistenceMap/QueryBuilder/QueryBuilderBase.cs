using PersistenceMap.QueryParts;
using PersistenceMap.Tracing;

namespace PersistenceMap.QueryBuilder
{
    public class QueryBuilderBase<TContext> : IQueryExpression where TContext : IDatabaseContext
    {
        public QueryBuilderBase(TContext context)
        {
            _context = context;
        }

        public QueryBuilderBase(TContext context, IQueryPartsContainer container)
        {
            _context = context;
            _queryParts = container;
        }
        
        #region IQueryProvider Implementation

        readonly TContext _context;
        public TContext Context
        {
            get
            {
                return _context;
            }
        }

        IDatabaseContext IQueryExpression.Context
        {
            get
            {
                return _context;
            }
        }

        IQueryPartsContainer _queryParts;
        public virtual IQueryPartsContainer QueryParts
        {
            get
            {
                if (_queryParts == null)
                    _queryParts = new QueryPartsContainer();
                return _queryParts;
            }
        }

        #endregion
    }
}
