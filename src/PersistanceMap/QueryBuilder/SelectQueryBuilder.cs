using PersistanceMap.QueryBuilder.QueryPartsBuilders;
using PersistanceMap.QueryParts;

namespace PersistanceMap.QueryBuilder
{
    public partial class SelectQueryBuilder<T> : SelectQueryPartsBuilder<T>, ISelectQueryProviderBase<T>, ISelectQueryProvider<T>, IJoinQueryProvider<T>, IWhereQueryProvider<T>, IAfterMapQueryProvider<T>, IQueryProvider
    {
        public SelectQueryBuilder(IDatabaseContext context)
        {
            _context = context;
        }

        public SelectQueryBuilder(IDatabaseContext context, SelectQueryPartsMap container)
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

        IQueryPartsMap IQueryProvider.QueryPartsMap
        {
            get
            {
                return QueryPartsMap;
            }
        }

        #endregion

        #region Internal Implementation

        internal ISelectQueryProvider<T2> From<T2>()
        {
            // create the begining for the select operation
            AppendSimpleQueryPart(QueryPartsMap, OperationType.Select);

            // add the from operation
            AppendEntityQueryPart<T2>(QueryPartsMap, OperationType.From);

            return new SelectQueryBuilder<T2>(Context, QueryPartsMap);
        }

        internal ISelectQueryProvider<T2> From<T2>(string alias)
        {
            alias.EnsureArgumentNotNullOrEmpty("alias");

            // create the begining for the select operation
            AppendSimpleQueryPart(QueryPartsMap, OperationType.Select);

            // add the from operation with a alias
            var part = AppendEntityQueryPart<T>(QueryPartsMap, OperationType.From);
            part.EntityAlias = alias;

            return new SelectQueryBuilder<T2>(Context, QueryPartsMap);
        }

        #endregion
    }
}
