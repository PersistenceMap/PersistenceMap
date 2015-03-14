using PersistanceMap.QueryBuilder;
using PersistanceMap.QueryBuilder.Commands;
using PersistanceMap.QueryParts;
using System.Text;

namespace PersistanceMap.SqlServer.QueryBuilder
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

            //var part = new DelegateQueryPart(OperationType.CreateDatabase, () => string.Format("CREATE DATABASE {0}", database));

            var sb = new StringBuilder();
            sb.AppendLine("DECLARE @device_directory NVARCHAR(520)");
            sb.AppendLine("SELECT @device_directory = SUBSTRING(filename, 1, CHARINDEX(N'master.mdf', LOWER(filename)) - 1)");
            sb.AppendLine("FROM master.dbo.sysaltfiles WHERE dbid = 1 AND fileid = 1");
            //sb.AppendLine(string.Format("EXECUTE (N'CREATE DATABASE {0} ON PRIMARY (NAME = N''Northwind'', FILENAME = N''' + @device_directory + N'{0}.mdf'') LOG ON (NAME = N''Northwind_log'',  FILENAME = N''' + @device_directory + N'{0}.ldf'')')", database));
            sb.AppendLine(string.Format("EXECUTE (N'CREATE DATABASE {0} ON PRIMARY (NAME = N''{0}'', FILENAME = N''' + @device_directory + N'{0}.mdf'') LOG ON (NAME = N''{0}_log'',  FILENAME = N''' + @device_directory + N'{0}.ldf'')')", database));

            var part = new DelegateQueryPart(OperationType.CreateDatabase, () => sb.ToString());
            QueryParts.Add(part);

            Context.AddQuery(new MapQueryCommand(QueryParts));


            var resetPart = new DelegateQueryPart(OperationType.None, () =>
            {
                // reset the connectionstring to the created
                Context.ConnectionProvider.Database = database;
                return string.Format("USE {0}", database);
            });
            var resetQueryMap = new QueryPartsContainer();
            resetQueryMap.Add(resetPart);
            Context.AddQuery(new MapQueryCommand(resetQueryMap));
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
