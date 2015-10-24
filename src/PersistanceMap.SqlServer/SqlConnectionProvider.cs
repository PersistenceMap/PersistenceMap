using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace PersistanceMap
{
    public class SqlConnectionProvider : ConnectionProvider, IConnectionProvider
    {
        public SqlConnectionProvider(string connectionString)
            : base(connectionString, conStr => new SqlConnection(conStr))
        {
            QueryCompiler = new SqlServer.QueryCompiler();
        }
        
        /// <summary>
        /// Executes a sql query without returning a resultset
        /// </summary>
        /// <param name="query"></param>
        public override void ExecuteNonQuery(string query)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var executer = connection.GetExecuter(query);
                executer.ExecuteNonQuery(connection, query);
            }
        }
    }

    internal static class SqlConnectionExtensions
    {
        public static IQueryExecuter GetExecuter(this SqlConnection connection, string query)
        {
            var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (regex.Match(query).Success)
            {
                return new TransactionedQueryExeuter();
            }

            return new QueryExecuter();
        }
    }

    internal interface IQueryExecuter
    {
        void ExecuteNonQuery(SqlConnection connection, string query);
    }

    internal class QueryExecuter : IQueryExecuter
    {
        public void ExecuteNonQuery(SqlConnection connection, string query)
        {
            using (var command = new SqlCommand(query, connection))
            {
                try
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    System.Diagnostics.Trace.WriteLine(e);
                    throw e;
                }
            }
        }
    }

    internal class TransactionedQueryExeuter : IQueryExecuter
    {
        public void ExecuteNonQuery(SqlConnection connection, string query)
        {
            // SqlCommand can't handle go breakes so split all go
            var regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string[] lines = regex.Split(query);

            var transaction = connection.BeginTransaction();
            
            using (var command = connection.CreateCommand())
            {
                try
                {
                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            command.CommandText = line;
                            command.Transaction = transaction;

                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SqlException e)
                {
                    transaction.Rollback();
                    System.Diagnostics.Trace.WriteLine(e);
                    throw e;
                }
            }

            transaction.Commit();
        }
    }
}
