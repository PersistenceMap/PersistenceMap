using System.Data.SqlServerCe;

namespace PersistenceMap
{
    public class SqlCeConnectionProvider : ConnectionProvider, IConnectionProvider
    {
        public SqlCeConnectionProvider(string connectionString)
            : base(connectionString, conStr => new SqlCeConnection(conStr))
        {
        }
    }
}
