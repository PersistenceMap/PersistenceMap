using PersistanceMap.QueryParts;
using System;

namespace PersistanceMap.Sqlite.QueryBuilder
{
    internal class DatabaseQueryBuilderBase : IQueryExpression
    {
        public DatabaseQueryBuilderBase(IDatabaseContext context)
        {
            _context = context;
        }

        public DatabaseQueryBuilderBase(IDatabaseContext context, IQueryPartsMap container)
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

    }

    internal class DatabaseQueryBuilder : DatabaseQueryBuilderBase, IDatabaseQueryExpression, IQueryExpression
    {
        public DatabaseQueryBuilder(IDatabaseContext context)
            : base(context)
        {
        }

        public DatabaseQueryBuilder(IDatabaseContext context, IQueryPartsMap container)
            : base(context, container)
        {
        }
        
        #region IDatabaseQueryExpression Implementation

        public virtual void Create()
        {
            throw new NotImplementedException();
        }

        public ITableQueryExpression<T> Table<T>()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class TableQueryBuilder<T> : DatabaseQueryBuilderBase, ITableQueryExpression<T>
    {
        public TableQueryBuilder(IDatabaseContext context, IQueryPartsMap container)
            : base(context, container)
        {
        }

        #region ITableQueryExpression Implementation

        public void Create()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public ITableQueryExpression<T> Ignore(System.Linq.Expressions.Expression<Func<T, object>> field)
        {
            throw new NotImplementedException();
        }

        public ITableQueryExpression<T> Key(params System.Linq.Expressions.Expression<Func<T, object>>[] keys)
        {
            throw new NotImplementedException();
        }

        public ITableQueryExpression<T> Key<TRef>(System.Linq.Expressions.Expression<Func<T, object>> field, System.Linq.Expressions.Expression<Func<TRef, object>> reference, string name = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
