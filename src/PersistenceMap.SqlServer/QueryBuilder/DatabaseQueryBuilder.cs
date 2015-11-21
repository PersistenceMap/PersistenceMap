using PersistenceMap.QueryBuilder;
using PersistenceMap.QueryBuilder.Commands;
using PersistenceMap.QueryParts;
using System.Text;

namespace PersistenceMap.SqlServer.QueryBuilder
{
    internal class DatabaseQueryBuilder : QueryBuilderBase<SqlDatabaseContext>, IDatabaseQueryExpression, IQueryExpression
    {
        public DatabaseQueryBuilder(SqlDatabaseContext context)
            : base(context)
        {
        }

        public DatabaseQueryBuilder(SqlDatabaseContext context, IQueryPartsContainer container)
            : base(context, container)
        {
        }

        #region IDatabaseQueryExpression Implementation

        /// <summary>
        /// Creates a create database expression
        /// </summary>
        public void Create()
        {
            var database = Context.ConnectionProvider.Database;
            var setPart = new DelegateQueryPart(OperationType.None, () =>
            {
                // set the connectionstring to master database
                Context.ConnectionProvider.Database = "Master";
                return string.Empty;
            });
            QueryParts.Add(setPart);

            var part = new DelegateQueryPart(OperationType.CreateDatabase, () => database);
            QueryParts.Add(part);

            Context.AddQuery(new MapQueryCommand(QueryParts));

            var resetPart = new DelegateQueryPart(OperationType.None, () =>
            {
                // reset the connectionstring to the created
                Context.ConnectionProvider.Database = database;
                return string.Format("USE {0}", database);
                //return string.Empty;
            });
            var resetQueryMap = new QueryPartsContainer();
            resetQueryMap.Add(resetPart);

            Context.AddQuery(new MapQueryCommand(resetQueryMap));
        }

        /// <summary>
        /// Detaches the database
        /// </summary>
        public void Detach()
        {
            var database = Context.ConnectionProvider.Database;
            QueryParts.Add(new DelegateQueryPart(OperationType.None, () =>
            {
                // set the connectionstring to master database
                Context.ConnectionProvider.Database = "Master";
                return string.Empty;
            }));
            QueryParts.Add(new DelegateQueryPart(OperationType.DetachDatabase, () => database));
            Context.AddQuery(new MapQueryCommand(QueryParts));
        }

        /// <summary>
        /// Drops the database
        /// </summary>
        public void Drop()
        {
            var database = Context.ConnectionProvider.Database;
            QueryParts.Add(new DelegateQueryPart(OperationType.None, () =>
            {
                // set the connectionstring to master database
                Context.ConnectionProvider.Database = "Master";
                return string.Empty;
            }));
            QueryParts.Add(new DelegateQueryPart(OperationType.DropDatabase, () => database));
            Context.AddQuery(new MapQueryCommand(QueryParts));
        }

        /// <summary>
        /// Creates a table expression
        /// </summary>
        /// <typeparam name="T">The POCO type defining the table</typeparam>
        /// <returns></returns>
        public ITableQueryExpression<T> Table<T>()
        {
            return new TableQueryBuilder<T, SqlDatabaseContext>(Context, QueryParts);
        }

        #endregion
    }
}
