using System.Data.SQLite;

namespace PersistanceMap
{
    public class SqliteConnectionProvider : ConnectionProvider, IConnectionProvider
    {
        public SqliteConnectionProvider(string connectionString) 
            : base(connectionString, conStr => new SQLiteConnection(conStr))
        {
            QueryCompiler = new Sqlite.QueryCompiler();
        }
    }
}
