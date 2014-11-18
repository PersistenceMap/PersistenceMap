using PersistanceMap.QueryParts;

namespace PersistanceMap.QueryBuilder
{
    public class QueryBuilderBase<TContext> : IQueryExpression where TContext : IDatabaseContext
    {
        public QueryBuilderBase(TContext context)
        {
            _context = context;
        }

        public QueryBuilderBase(TContext context, IQueryPartsMap container)
        {
            _context = context;
            _queryPartsMap = container;
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
    }
}
