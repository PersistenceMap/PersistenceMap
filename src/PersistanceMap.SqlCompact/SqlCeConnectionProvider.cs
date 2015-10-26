using System.Data.SqlServerCe;

namespace PersistanceMap
{
    public class SqlCeConnectionProvider : ConnectionProvider, IConnectionProvider
    {
        public SqlCeConnectionProvider(string connectionString)
            : base(connectionString, conStr => new SqlCeConnection(conStr))
        {
        }
    }
}
