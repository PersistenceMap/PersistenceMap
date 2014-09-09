
namespace PersistanceMap.QueryProvider
{
    /// <summary>
    /// Containes logic to create queries that delete data from a database
    /// </summary>
    public class DeleteQueryProvider : IDeleteQueryProvider, IQueryProvider
    {
        public DeleteQueryProvider(IDatabaseContext context)
        {
            _context = context;
        }

        public DeleteQueryProvider(IDatabaseContext context, SelectQueryPartsMap container)
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

        //SelectQueryPartsMap _queryPartsMap;
        //public SelectQueryPartsMap QueryPartsMap
        //{
        //    get
        //    {
        //        if (_queryPartsMap == null)
        //            _queryPartsMap = new SelectQueryPartsMap();
        //        return _queryPartsMap;
        //    }
        //}

        //IQueryPartsMap IQueryProvider.QueryPartsMap
        //{
        //    get
        //    {
        //        return QueryPartsMap;
        //    }
        //}

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
    }
}
